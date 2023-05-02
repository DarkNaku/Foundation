using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkNaku.Foundation {
    public class CoroutineStream {
        public bool Stoped => _stoped;
        public bool Paused => _paused;
        public bool Completed => _completed;

        private bool _stoped = false;
        private bool _paused = false;
        private CSTask _current = null;
        private bool _completed = false;
        private Queue<CSTask> _tasks = null;
        private Coroutine _coroutine = null;
        private MonoBehaviour _player = null;
        private System.Action _onComplete = null;

        public CoroutineStream(MonoBehaviour player) {
            Debug.Assert(player != null, "[CoroutineStream] Constructor : Player is null.");
            _player = player;
            _tasks = new Queue<CSTask>();
            _coroutine = _player.StartCoroutine(CoPlay());
        }

        public CoroutineStream Pause() {
            _paused = true;

            if (_current != null) {
                _current.Pause();
            }

            return this;
        }

        public CoroutineStream Resume() {
            _paused = false;

            if (_current != null) {
                _current.Resume();
            }
            
            return this;
        }

        public void Stop() {
            _stoped = true;
            _tasks.Clear();

            if (_current != null) {
                _current.Stop();
                _current = null;
            }

            if (_coroutine != null) {
                _player.StopCoroutine(_coroutine);
            }
        }

        public CoroutineStream Append(params IEnumerator[] coroutines) {
            _tasks.Enqueue(new CSTask(_player, coroutines));
            return this;
        }

        public CoroutineStream Interval(float seconds) {
            _tasks.Enqueue(new CSTask(_player, WaitForSeconds(seconds)));
            return this;
        }

        public CoroutineStream Until(System.Func<bool> predicate) {
            _tasks.Enqueue(new CSTask(_player, WaitUntil(predicate)));
            return this;
        }

        public CoroutineStream While(System.Func<bool> predicate) {
            _tasks.Enqueue(new CSTask(_player, WaitWhile(predicate)));
            return this;
        }

        public CoroutineStream Callback(System.Action callback) {
            _tasks.Enqueue(new CSTask(_player, CoCallback(callback)));
            return this;
        }

        public CoroutineStream OnComplete(System.Action callback) {
            _onComplete = callback;
            return this;
        }

        public CoroutineStream Log(string message) {
            _tasks.Enqueue(new CSTask(_player, CoLog(message)));
            return this;
        }

        public CoroutineStream LogFormat(string message, params object[] args) {
            _tasks.Enqueue(new CSTask(_player, CoLogFormat(message, args)));
            return this;
        }

        public CoroutineStream LogWarning(string message) {
            _tasks.Enqueue(new CSTask(_player, CoLogWarning(message)));
            return this;
        }

        public CoroutineStream LogWarningFormat(string message, params object[] args) {
            _tasks.Enqueue(new CSTask(_player, CoLogWarningFormat(message, args)));
            return this;
        }

        public CoroutineStream LogError(string message) {
            _tasks.Enqueue(new CSTask(_player, CoLogError(message)));
            return this;
        }

        public CoroutineStream LogErrorFormat(string message, params object[] args) {
            _tasks.Enqueue(new CSTask(_player, CoLogErrorFormat(message, args)));
            return this;
        }

        private IEnumerator CoPlay() {
            yield return null;

            while (_tasks.Count > 0) {
                while (_paused) {
                    yield return null;
                }

                _current = _tasks.Dequeue();
                yield return _current.Play();
            }

            _tasks.Clear();
            _current = null;
            _completed = true;
            _onComplete?.Invoke();
        }

        private IEnumerator WaitForSeconds(float seconds) {
            yield return new CustomWaitForSeconds(seconds);
        }

        private IEnumerator WaitUntil(System.Func<bool> predicate) {
            yield return new WaitUntil(predicate);
        }

        private IEnumerator WaitWhile(System.Func<bool> predicate) {
            yield return new WaitWhile(predicate);
        }

        private IEnumerator CoCallback(System.Action callback) {
            callback?.Invoke();
            yield break;
        }

        private IEnumerator CoLog(string message) {
            Debug.Log(message);
            yield break;
        }

        private IEnumerator CoLogFormat(string message, params object[] args) {
            Debug.LogFormat(message, args);
            yield break;
        }

        private IEnumerator CoLogWarning(string message) {
            Debug.LogWarning(message);
            yield break;
        }

        private IEnumerator CoLogWarningFormat(string message, params object[] args) {
            Debug.LogWarningFormat(message, args);
            yield break;
        }

        private IEnumerator CoLogError(string message) {
            Debug.LogError(message);
            yield break;
        }

        private IEnumerator CoLogErrorFormat(string message, params object[] args) {
            Debug.LogErrorFormat(message, args);
            yield break;
        }
    }

    public class CustomWaitForSeconds : CustomYieldInstruction {
        public override bool keepWaiting => Time.time - _startTime < _seconds;

        private float _startTime = 0f;
        private float _seconds = 0f;

        public CustomWaitForSeconds(float seconds) {
            _seconds = seconds;
        }
    }
}