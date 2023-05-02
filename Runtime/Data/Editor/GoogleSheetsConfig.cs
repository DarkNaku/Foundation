using System;
using System.Net;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using UnityEditor;
using SimpleJSON;
using System.IO;
using OfficeOpenXml;
using System.Linq;

namespace DarkNaku.Foundation
{
    public enum FIELD_TYPE
    {
        UNDEFINED = 0,
        DOUBLE = 1,
        FLOAT = 2,
        INT = 4,
        LONG = 8,
        UINT = 16,
        ULONG = 32,
        BOOL = 64,
        STRING = 128,
    }

    public class GoogleAccessTokenResponse
    {
        public string access_token;
        public string refresh_token;
        public string scope;
        public string token_type;
        public int expires_in;
    }

    public class SpreadSheetData
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public string Link { get; private set; }

        public SpreadSheetData(string id, string name, string link)
        {
            ID = id;
            Name = name;
            Link = link;
        }
    }

    public class FieldInfo
    {
        private const string LIST_PREFIX = "list_";

        public string Name => _name;
        public FIELD_TYPE FieldType => _fieldType;
        public string FieldTypeNmae => _fieldTypeNmae;
        public bool IsList => _isList;
        public string Column => _column;
        public string Row => _row;
        public string Address => _column + _row;

        private string _name;
        private FIELD_TYPE _fieldType;
        private string _fieldTypeNmae;
        private bool _isList;
        private string _column;
        private string _row;

        public FieldInfo(ExcelRangeBase nameCell, ExcelRangeBase typeCell)
        {
            var fieldTypeInfo = GetFieldTypeInfo(typeCell);

            _name = nameCell.Text;
            _fieldType = fieldTypeInfo.Item1;
            _isList = fieldTypeInfo.Item2;
            _fieldTypeNmae = fieldTypeInfo.Item3;
            _column = typeCell.GetExcelColumn();
            _row = Mathf.Max(nameCell.End.Row, typeCell.End.Row).ToString();
        }

        private (FIELD_TYPE, bool, string) GetFieldTypeInfo(ExcelRangeBase typeCell)
        {
            var isList = false;
            var text = typeCell.Text;

            if (text.StartsWith(LIST_PREFIX))
            {
                isList = true;
                text = text.Replace(LIST_PREFIX, string.Empty);
            }

            return ((FIELD_TYPE)Enum.Parse(typeof(FIELD_TYPE), text, true), isList, text);
        }

        public override string ToString()
        {
            return string.Format("Name : {0}, FieldType : {1}, FieldTypeName : {2}, IsList : {3}, Column : {4}, Row : {5}",
                Name, FieldType, FieldTypeNmae, IsList, Column, Row);
        }
    }

    public class GoogleSheetsConfig : SingletonScriptable<GoogleSheetsConfig>
    {
        private const string AUTH_URL = "https://accounts.google.com/o/oauth2/auth";
        private const string TOKEN_URL = "https://oauth2.googleapis.com/token";
        private const string FILE_QUERY_URL = "https://www.googleapis.com/drive/v3/files";
        private const string FILE_DOWNLOAD_URL = "https://www.googleapis.com/drive/v3/files";
        private const string REDIRECT_URI = "http://localhost:8080";
        private const string SCOPE = "https://www.googleapis.com/auth/drive";

        private const string IGNORE_TOKEN = "ignore";
        private const string IGNORE_TOKEN2 = "//";
        private const string KEYS_RANGE = "a:a";
        private const string FIELD_NAME_TOKEN = "field_names";
        private const string FIELD_TYPE_TOKEN = "field_types";


        [SerializeField] private string _clientID;
        [SerializeField] private string _clientSecret;
        [SerializeField] private string _accessToken;
        [SerializeField] private string _refreshToken;
        [SerializeField] private long _expireTime;
        [SerializeField] private string _spreadSheetID;

        public static string ClientID
        {
            get => Instance._clientID;
            set => Instance._clientID = value;
        }

        public static string ClientSecret
        {
            get => Instance._clientSecret;
            set => Instance._clientSecret = value;
        }

        public static string AccessToken => Instance._accessToken;
        public static string RefreshToken => Instance._refreshToken;
        public static long ExpireTime => Instance._expireTime;
        public static bool IsAccessTokenExpired => Instance._IsAccessTokenExpired;
        public static bool IsAbleToRequestAccessToken => !string.IsNullOrEmpty(ClientID) && !string.IsNullOrEmpty(ClientSecret);
        public static bool IsAbleToRefreshAccessToken => 
            !string.IsNullOrEmpty(ClientID) && 
            !string.IsNullOrEmpty(ClientSecret) &&
            !string.IsNullOrEmpty(RefreshToken) &&
            IsAccessTokenExpired;

        public static bool HasAccessToken => !string.IsNullOrEmpty(Instance._accessToken);
        public static bool IsSelectedSpreadSheetID => !string.IsNullOrEmpty(Instance._spreadSheetID);

        public static List<string> SpreadSheetNames => Instance._spreadSheetNames;
        public static List<SpreadSheetData> SpreadSheetDatas => Instance._spreadSheetDatas;

        private bool _IsAccessTokenExpired => DateTime.Now >= new DateTime(_expireTime);

        private List<string> _spreadSheetNames = new();
        private List<SpreadSheetData> _spreadSheetDatas = new();

        public static void RequestAccessToken()
        {
            Instance._RequestAccessToken();
        }

        public static void RefreshAccessToken()
        {
            EditorCoroutineUtility.StartCoroutine(Instance.CoRefreshAccessToken(), Instance);
        }

        public static void RequestSpreadSheets()
        {
            EditorCoroutineUtility.StartCoroutine(Instance.CoRequestSpreadSheets(), Instance);
        }

        public static void SelectSpreadSheet(int index)
        {
            Instance._SelectSpreadSheet(index);
        }

        public static void DownloadSpreadSheet()
        {
            EditorCoroutineUtility.StartCoroutine(Instance.CoDownloadSpreadSheet(), Instance);
        }

        private void _RequestAccessToken()
        { 
            // HttpListener 객체 생성 및 시작
            var httpListener = new HttpListener();
            httpListener.Prefixes.Add(REDIRECT_URI + "/");
            httpListener.Start();
            httpListener.BeginGetContext(OnBeginGetContext, httpListener);

            var codeRequestURL = AUTH_URL + "?" +
                "redirect_uri=" + REDIRECT_URI + "&" +
                "client_id=" + _clientID + "&" +
                "scope=" + SCOPE + "&" +
                "response_type=code&" +
                "access_type=offline";

            Application.OpenURL(codeRequestURL);
	    }

        private void OnBeginGetContext(IAsyncResult result)
        {
            var httpListener = result.AsyncState as HttpListener;

            if (httpListener == null) {
                Debug.LogError("[GoogleSheetsConfig] HttpListener is null.");
                return;
	        }

            try
            {
                var context = httpListener.EndGetContext(result);

                Response(context);

                var code = context.Request.QueryString["code"];

                if (code == null)
                {
                    Debug.LogError("[GoogleSheetsConfig] Code is null.");
                    return;
                }

                httpListener.Stop();
                httpListener.Close();

                EditorCoroutineUtility.StartCoroutine(CoRequestAccessToken(code), this);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("[GoogleSheetsConfig] {0}", e.Message);
	        }
        }

        private void Response(HttpListenerContext context)
        {
            byte[] buffer = Encoding.UTF8.GetBytes("<h1>Linked to Google sheet. Close the window and return to Unity.</h1>");
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "text/html";
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
	    }

        private IEnumerator CoRequestAccessToken(string code)
        {
            var param = new WWWForm();
            param.AddField("code", code);
            param.AddField("client_id", _clientID);
            param.AddField("client_secret", _clientSecret);
            param.AddField("redirect_uri", REDIRECT_URI);
            param.AddField("scope", SCOPE);
            param.AddField("grant_type", "authorization_code");

            using (var request = UnityWebRequest.Post(TOKEN_URL, param))
            {
                yield return request.SendWebRequest();

                var response = JsonUtility.FromJson<GoogleAccessTokenResponse>(request.downloadHandler.text);
                _accessToken = response.access_token;
                _refreshToken = response.refresh_token;
                _expireTime = DateTime.Now.Add(TimeSpan.FromSeconds(response.expires_in)).Ticks;

                EditorUtility.SetDirty(GoogleSheetsConfig.Instance);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private IEnumerator CoRefreshAccessToken()
        {
            var param = new WWWForm();
            param.AddField("client_id", _clientID);
            param.AddField("client_secret", _clientSecret);
            param.AddField("refresh_token", _refreshToken);
            param.AddField("grant_type", "refresh_token");

            using (var request = UnityWebRequest.Post(TOKEN_URL, param))
            {
                yield return request.SendWebRequest();

                var response = JsonUtility.FromJson<GoogleAccessTokenResponse>(request.downloadHandler.text);
                _accessToken = response.access_token;
                _expireTime = DateTime.Now.Add(TimeSpan.FromSeconds(response.expires_in)).Ticks;

                EditorUtility.SetDirty(GoogleSheetsConfig.Instance);
                AssetDatabase.SaveAssets();
            }
        }

        private IEnumerator CoRequestSpreadSheets()
        {
            if (IsAbleToRefreshAccessToken)
            {
                yield return CoRefreshAccessToken();
            }

            string url = FILE_QUERY_URL + "?" + 
                "q=" + Uri.EscapeDataString("mimeType='application/vnd.google-apps.spreadsheet'") + 
                "&corpora=user" +
                "&includeItemsFromAllDrives=true" +
                "&supportsAllDrives=true" + 
                "&fields=files(exportLinks,name,webContentLink,mimeType,id,trashed)" +
                "&access_token=" + AccessToken;

            using (var request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();
                
                Debug.Log(request.downloadHandler.text);

                _spreadSheetNames.Clear();

                var json = JSON.Parse(request.downloadHandler.text);

                var files = json["files"];

                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];

                    var id = file["id"].Value;
                    var name = file["name"].Value;
                    var exportLink = file["exportLinks"]["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"].Value;
                    _spreadSheetNames.Add(file["name"].Value);
                    _spreadSheetDatas.Add(new SpreadSheetData(id, name, exportLink));
                }
            }
        }

        private void _SelectSpreadSheet(int index)
        {
            if ((index >= 0) && (index < _spreadSheetDatas.Count))
            {
                _spreadSheetID = _spreadSheetDatas[index].ID;
            }
        }

        private IEnumerator CoDownloadSpreadSheet()
        {
            if (IsAbleToRefreshAccessToken)
            {
                yield return CoRefreshAccessToken();
            }

            var localPath = $"{Application.temporaryCachePath}/{_spreadSheetID}.xlsx";
            string url = $"{FILE_DOWNLOAD_URL}/{_spreadSheetID}/export?mimeType=application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using(var request = UnityWebRequest.Get(url)) {
                request.SetRequestHeader("authorization", "Bearer " + AccessToken);
                request.SetRequestHeader("accept", "application/json");

                yield return request.SendWebRequest();

                if (string.IsNullOrEmpty(request.error))
                {
                    Debug.Log(localPath);
                    File.WriteAllBytes(localPath, request.downloadHandler.data);

                    var schemas = new Dictionary<string, List<FieldInfo>>();
                    var fileInfo = new FileInfo(localPath);
                    var excelPackage = new ExcelPackage(fileInfo);
                    var sheetNames = GetSheetNames(ref excelPackage);

                    for (int i = 0; i < sheetNames.Count; i++) {
                        schemas.Add(sheetNames[i], GetFieldInfos(ref excelPackage, sheetNames[i]));

                        var addresses = GetExcelKeyAddresses(ref excelPackage, sheetNames[i]);

                        for (int j = 0; j < addresses.Count; j++)
                        {
                            Debug.Log(addresses[j].Address);
                        }
                    }

                    ClassGenerator.Generate(schemas);
                }
            }
        }

        private List<string> GetSheetNames(ref ExcelPackage excelPackage)
        {
            if (excelPackage == null) return Enumerable.Empty<string>().ToList();

            List<string> sheetNames = new List<string>();

            foreach (var sheet in excelPackage.Workbook.Worksheets)
            {
                var lowerName = sheet.Name.Trim().ToLower();

                if (lowerName.StartsWith(IGNORE_TOKEN)) continue;
                if (lowerName.StartsWith(IGNORE_TOKEN2)) continue;

                sheetNames.Add(sheet.Name);
            }

            return sheetNames;
        }

        public ExcelRange GetSheetCells(ref ExcelPackage excelPackage, string sheetName)
        {
            if (excelPackage == null) return null;
            if (string.IsNullOrEmpty(sheetName)) return null;

            return excelPackage.Workbook.Worksheets[sheetName]?.Cells;
        }

        private List<FieldInfo> GetFieldInfos(ref ExcelPackage excelPackage, string sheetName)
        {
            if (excelPackage == null || string.IsNullOrEmpty(sheetName)) {
                return Enumerable.Empty<FieldInfo>().ToList();
            }

            var fieldInfos = new List<FieldInfo>();
            var sheetCells = GetSheetCells(ref excelPackage, sheetName);
            var fieldNameCell = GetFieldNameCellAddress(ref excelPackage, sheetName);
            var fieldTypeCell = GetFieldTypeCellAddress(ref excelPackage, sheetName);

            if (fieldNameCell == null || fieldTypeCell == null) {
                return Enumerable.Empty<FieldInfo>().ToList();
            }

            var nameRow = sheetCells["b"+fieldNameCell.Row+":zz"+fieldNameCell.Row];
            
            foreach (var nameCell in nameRow)
            {
                var address = nameCell.GetExcelColumn() + (nameCell.End.Row + 1).ToString();
                var typeCell = sheetCells[address];

                fieldInfos.Add(new FieldInfo(nameCell, typeCell));
            }

            return fieldInfos;
        }

        public ExcelCellAddress GetFieldNameCellAddress(ref ExcelPackage excelPackage, string name)
        {
            var fieldNameCellAddress = GetExcelCellAddress(ref excelPackage, name, FIELD_NAME_TOKEN);

            if (fieldNameCellAddress == null)
            {
                Debug.LogWarningFormat("[GoogleSheets] GetFieldNameCellAddress : Can not found field name cell - {0}", FIELD_NAME_TOKEN);
            }

            return fieldNameCellAddress;
        }

        public ExcelCellAddress GetFieldTypeCellAddress(ref ExcelPackage excelPackage, string name)
        {
            var fieldTypeCellAddress = GetExcelCellAddress(ref excelPackage, name, FIELD_TYPE_TOKEN);

            if (fieldTypeCellAddress == null)
            {
                Debug.LogWarningFormat("[GoogleSheets] GetFieldTypeCellAddress : Can not found field type cell - {0}", FIELD_TYPE_TOKEN);
            }

            return fieldTypeCellAddress;
        }

        ExcelCellAddress GetExcelCellAddress(ref ExcelPackage excelPackage, string name, string token)
        {
            if (excelPackage == null) return null;
            if (string.IsNullOrEmpty(name)) return null;

            ExcelWorksheet sheet = excelPackage.Workbook.Worksheets[name];

            if (sheet == null) return null;

            var cell = sheet.Cells[KEYS_RANGE].FirstOrDefault(cell => 
                cell != null && 
                !string.IsNullOrEmpty(cell.Text) && 
                cell.Text.Trim().ToLower().Equals(token));

            return (cell == null) ? null : new ExcelCellAddress(cell.Address);
        }

        List<ExcelCellAddress> GetExcelKeyAddresses(ref ExcelPackage excelPackage, string name)
        {
            if (excelPackage == null) return null;
            if (string.IsNullOrEmpty(name)) return null;

            ExcelWorksheet sheet = excelPackage.Workbook.Worksheets[name];

            if (sheet == null) return null;

            var addresses = sheet.Cells[KEYS_RANGE]
                .Where(cell => cell != null && !string.IsNullOrEmpty(cell.Text))
                .Select(cell => new ExcelCellAddress(cell.Address));

            return addresses.ToList();
        }
    }
}