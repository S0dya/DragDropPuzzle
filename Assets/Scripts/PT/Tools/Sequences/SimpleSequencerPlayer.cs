using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PT.Tools.Sequences
{
    public class SimpleSequencerPlayer : MonoBehaviour
    {
        [SerializeField] private Sequencer sequencer;

        private void Start()
        {
            sequencer.Play().Forget();

            enabled = false;
        }
    }
}