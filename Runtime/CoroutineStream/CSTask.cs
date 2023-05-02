using System.Collections;
using UnityEngine;

namespace DarkNaku.Foundation {
    public class CSTask : CustomYieldInstruction {
        public bool Stoped => _stoped;
        public bool Paused => _paused;
        public bool Completed => _completeCount == _taskCount;
        public override bool keepWaiting => !Completed;

        private int _taskCount = 0;
        private int _completeCount = 0;
        private bool _stoped = false;
        private bool _paused = false;
        private bool _running = false;
        private Coroutine _coroutine = null;
        private IEnumerator[] _tasks = null;
        private MonoBehaviour _player = null;

        public CSTask(MonoBehaviour player, params IEnumerator[] tasks) {
            Debug.Assert(player != null, "[Task] Constructor : Player is null.");
            Debug.Assert((tasks != null) && (tasks.Length > 0), "[Task] Constructor : It must have at least one parameter.");

            _player = player;
            _tasks = tasks;
            _taskCount = _tasks.Length;
        }

        public Coroutine Play() {
            _coroutine = _player.StartCoroutine(CoPlay(_player));
            return _coroutine;
        }

        public void Pause() {
            _paused = true;
        }

        public void Resume() {
            _paused = false;
        }

        public void Stop() {
            _tasks = null;
            _stoped = true;
            _running = false;
            _player.StopCoroutine(_coroutine);
        }

        private IEnumerator CoPlay(MonoBehaviour player) {
            _running = true;

            for (int i = 0; i < _tasks.Length; i++) {
                player.StartCoroutine(CoWrapper(_tasks[i]));
            }

            yield return new WaitUntil(() => Completed);

            _tasks = null;
        }

        private IEnumerator CoWrapper(IEnumerator coroutine) {
            if (coroutine == null) {
                _completeCount++;
                yield break;
            }

            while (_running) {
                if (_paused) {
                    yield return null;
                } else if (coroutine.MoveNext()) {
                    yield return coroutine.Current;
                } else {
                    _completeCount++;
                    break;
                }
            }
        }
    }
}