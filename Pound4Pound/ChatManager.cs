using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ChatManager : MonoBehaviourPun
{
    bool isChatting = false;
    string chatInput = "";

    [System.Serializable]
    public class ChatMessage
    {
        public string sender = "";
        public string message = "";
        public float timer = 0;
    }

    List<ChatMessage> chatMessages = new List<ChatMessage>();

    // Start is called before the first frame update
    void Start()
    {
        //Initialize Photon View
        if(gameObject.GetComponent<PhotonView>() == null)
        {
            PhotonView photonView = gameObject.AddComponent<PhotonView>();
            photonView.ViewID = 1;
        }
        else
        {
            photonView.ViewID = 1;
        }
        isChatting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !isChatting)
        {
            Debug.Log("Chatting...");
            isChatting = true;
            chatInput = "";
        }

        //Hide messages after timer is expired
        for (int i = 0; i < chatMessages.Count; i++)
        {
            if (chatMessages[i].timer > 0)
            {
                chatMessages[i].timer -= Time.deltaTime;
            }
        }
    }
    public GUIStyle inputStyle;
    void OnGUI()
    {
        if (!isChatting)
        {
            // Debug.Log("Not Chatting");
            GUI.Label(new Rect(30, Screen.height - 60, 400, 30), "Press 'Enter' to chat", inputStyle);
        }
        else
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                isChatting = false;
                if(chatInput.Replace(" ", "") != "")
                {
                    //Send message
                    photonView.RPC("SendChat", RpcTarget.All, PhotonNetwork.LocalPlayer, chatInput);
                }
                chatInput = "";
            }

            GUI.SetNextControlName("ChatField");
            GUI.Label(new Rect(30, Screen.height - 60, 400, 30), "Say:", inputStyle);
            // GUIStyle inputStyle = GUI.skin.GetStyle("box");
            inputStyle.alignment = TextAnchor.MiddleLeft;
            chatInput = GUI.TextField(new Rect(100, Screen.height - 60, 400, 30), chatInput, 60, inputStyle);

            GUI.FocusControl("ChatField");
        }
        
        //Show messages
        for(int i = 0; i < chatMessages.Count; i++)
        {
            if(chatMessages[i].timer > 0 || isChatting)
            {
                GUI.Label(new Rect(30, Screen.height - 90 - 30 * i, 500, 30), chatMessages[i].sender + ": " + chatMessages[i].message, inputStyle);
            }
        } 
    }

    [PunRPC]
    void SendChat(Player sender, string message)
    {
        ChatMessage m = new ChatMessage();
        m.sender = sender.NickName;
        m.message = message;
        m.timer = 8.0f;

        chatMessages.Insert(0, m);
        if(chatMessages.Count > 8)
        {
            chatMessages.RemoveAt(chatMessages.Count - 1);
        }
    }
}















// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;
// using Photon.Pun;


// [RequireComponent(typeof(PhotonView))]
// public class ChatManager : MonoBehaviourPunCallbacks, IPunObservable
// {
//     public Rect GuiRect = new Rect(20, 30, 580, 400);
//     public bool IsVisible = true;
//     public bool AlignBottom = false;
//     public List<string> messages = new List<string>();
//     private string inputLine = "";
//     private Vector2 scrollPos = Vector2.zero;

//     public static readonly string ChatRPC = "Chat";

//     public void Start()
//     {
//         scrollPos.y = Mathf.Infinity;
//         if (this.AlignBottom)
//         {
//             this.GuiRect.y = Screen.height - this.GuiRect.height;
//         }
//     }

//     public void OnGUI()
//     {
//         GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = 24;

//         if (!this.IsVisible || !PhotonNetwork.InRoom)
//         {
//             return;
//         }

//         if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
//         {
//             if (!string.IsNullOrEmpty(this.inputLine))
//             {
//                 this.photonView.RPC("Chat", RpcTarget.AllViaServer, this.inputLine);
//                 this.inputLine = "";
//                 GUI.FocusControl("");
//                 return; // printing the now modified list would result in an error. to avoid this, we just skip this single frame
//             }
//             else
//             {
//                 GUI.FocusControl("ChatInput");
//             }
//         }

//         GUI.SetNextControlName("");
//         GUILayout.BeginArea(this.GuiRect);

//         scrollPos = GUILayout.BeginScrollView(scrollPos);
//         GUILayout.FlexibleSpace();

//         //for (int i = messages.Count - 1; i >= 0; i--)
//         //{
//         // GUILayout.Label(messages[i]);
//         //}

//         for (int i = 0; i < messages.Count; i++)
//         {
//             GUILayout.Label(messages[i]);

//             if (messages.Count >= 10)
//             {
//                 this.messages.RemoveAt(0);
//             }
//         }

//         GUILayout.EndScrollView();

//         GUILayout.BeginHorizontal();
//         GUI.SetNextControlName("ChatInput");
//         inputLine = GUILayout.TextField(inputLine, GUILayout.Height(50), GUILayout.Width(440));
//         if (GUILayout.Button("Send", GUILayout.Height(50), GUILayout.Width(120), GUILayout.ExpandWidth(false)))
//         {
//             Debug.Log("Pressed send msg button");
//             this.photonView.RPC("Chat", RpcTarget.AllViaServer, this.inputLine);
//             this.inputLine = "";
//             GUI.FocusControl("");
//         }
//         GUILayout.EndHorizontal();
//         GUILayout.EndArea();
//     }

//     [PunRPC]
//     public void Chat(string newLine, PhotonMessageInfo mi)
//     {
//         string senderName = "anonymous";

//         if (mi.Sender != null)
//         {
//             if (!string.IsNullOrEmpty(mi.Sender.NickName))
//             {
//                 senderName = mi.Sender.NickName;
//             }
//             else
//             {
//                 senderName = "player " + mi.Sender.NickName;
//             }
//         }
//         this.messages.Add(senderName + ": " + newLine);
//         Debug.Log("Sending message...");
//     }

//     [PunRPC]
//     public void Broadcast(string broadcast)
//     {
//         AddLine(broadcast);
//     }

//     public void AddLine(string newLine)
//     {
//         this.messages.Add(newLine);
//     }

//     public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//     {
//         throw new System.NotImplementedException();
//     }
// }

