using UnityEngine;

namespace client
{
	/**
     * Empty scaffold for you to build upon for your chatbox.
     * Read through this code and the screen wrappers and then decide either to use this
     * or roll your own
	 */
	class TcpChatClient : MonoBehaviour
    {

        [SerializeField]
        private ConnectPanel _connectPanel;
        [SerializeField]
        private NameChoosePanel _nameChoosePanel;
        [SerializeField]
        private MainChatPanel _chatPanel;

		//stores the panel, panel corresponds directly one to one with a state...
        private BasePanel _currentState;

        void Awake()
        {
            _connectPanel.DisableGUI();
            _nameChoosePanel.DisableGUI();
            _chatPanel.DisableGUI();

            //switch to initial state
            changeState(_connectPanel);
        }

        /**
		 * Brute force change state method, detaches/attaches event handlers where required
		 * and shows the correct interface. This does not take care of (dis)connecting etc, 
		 * that should be taken care of in the handlers attached to each screen.
		 * 
		 * Better way is a state machine of course.
		 */
		private void changeState (BasePanel pPanel) {
			//ignore faulty state changes
			if (_currentState == pPanel) return;

			//disable components
			if (_currentState != null) {
				_currentState.DisableGUI();
			}

			//remove correct listeners
			if (_currentState == _connectPanel) {
                _connectPanel.UnregisterButtonConnectClickActions();
			} else if (_currentState == _nameChoosePanel) {
                _nameChoosePanel.UnregisterButtonJoinClickActions();
            } else if (_currentState == _chatPanel) {
			    _chatPanel.UnregisterButtonSendClickActions();
				_chatPanel.UnregisterButtonDisconnectClickActions();
				//_chatPanel.OnUserClicked -= onChatPanelUserClicked;
			} 

			//set new state
			_currentState = pPanel;

			//set new listeners
			if (_currentState == _connectPanel) {
                _connectPanel.RegisterButtonConnectClickAction(onConnectPanelConnectButtonClicked);
                _connectPanel.SetStatusbarText("");
            } else if (_currentState == _nameChoosePanel) {
				_nameChoosePanel.SetStatusbarText("");
				_nameChoosePanel.RegisterButtonJoinClickAction(onNameChoosePanelButtonJoinClicked);
			} else if (_currentState == _chatPanel) {
				_chatPanel.RegisterButtonSendClickAction(onChatPanelEntryActivated);
				_chatPanel.RegisterButtonDisconnectClickAction(onChatPanelButtonLeaveClicked);
				//_chatPanel.OnUserClicked += onChatPanelUserClicked;
			} 

			//enable and show new state
			if (_currentState != null) {
                _currentState.EnableGUI();
			}
		}

        private void Update()
        {
            //whether this is needed depends on whether you are using polling or threads...
            //if (_currentState == _chatPanel) onChatPanelStateUpdate();
        }

        /************************ CONNECT STATE EVENTS *****************/

        private void onConnectPanelConnectButtonClicked () {
            Debug.Log("onConnectPanelConnectButtonClicked");

            //fake action for now:
            changeState(_nameChoosePanel);
        }

		/************************ NAME CHOOSE STATE EVENTS *****************/

		private void onNameChoosePanelButtonJoinClicked () {
			//check if everything is filled in correctly
			if (_nameChoosePanel.Validate ()) {
                Debug.Log("OK: Name filled in:" + _nameChoosePanel.GetNickName());

                //fake action for now:
                changeState(_chatPanel);
			}
            else
            {
                Debug.Log("!OK: Name filled in:" + _nameChoosePanel.GetNickName());

                //todo provide feedback to user
            }
		}

		/************************ CHAT STATE EVENTS *****************/

		private void onChatPanelEntryActivated ()
		{
			string messageToSend = _chatPanel.GetChatEntry();
			if (messageToSend.Length == 0) return;

            Debug.Log("Entry to process:" + messageToSend);

            _chatPanel.SetChatEntry("");
		}

		private void onChatPanelButtonLeaveClicked() {
			changeState (_connectPanel);
			_chatPanel.RemoveAllUsers ();
			_chatPanel.ClearAllText();
		}

    }
}
