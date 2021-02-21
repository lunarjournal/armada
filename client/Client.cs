using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace Test
{

	public class Hack : MonoBehaviour
	{
		struct Log
		{
			public string message;
			public string stackTrace;
			public LogType type;
		}

		public KeyCode toggleKey = KeyCode.BackQuote;

		List<Log> logs = new List<Log>();
		Vector2 scrollPosition;
		bool show;
		bool speedhack;
		bool salt;
		GameObject g_obj = null; 
		bool collision;
		List<GameObject> tesla = new List<GameObject>();
		bool collapse;
		Vector3 position = new Vector3(0,0,0);
		int myId;

		static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
		{
			{ LogType.Assert, Color.white },
				{ LogType.Error, Color.red },
				{ LogType.Exception, Color.red },
				{ LogType.Log, Color.white },
				{ LogType.Warning, Color.yellow },
		};

		const int margin = 20;

		Rect windowRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));
		Rect titleBarRect = new Rect(0, 0, 10000, 20);
		GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

	
		void OnEnable ()
		{
			Application.RegisterLogCallback(HandleLog);
		}

		void OnDisable ()
		{
			Application.RegisterLogCallback(null);
		}

		
		void Update ()
		{

			 VehicleController[] myItems = FindObjectsOfType(typeof(VehicleController)) as VehicleController[];
			 foreach(VehicleController item in myItems){
			 	if(item.IsMine && item.IsAvailable){
			 		myId  = item.data.playerId;
			 	}
			 }

	
			if (Input.GetKeyDown(toggleKey)) {
				show = !show;	
			}

			if(Input.GetKeyDown(KeyCode.F1)){
				Debug.Log("Toggle [Speedhack]");
				speedhack =  !speedhack;
				if(speedhack == true){
					Time.timeScale = 5;
				}else{
					Time.timeScale = 1;
				}
			}

			if(Input.GetKeyDown(KeyCode.F2)){

			Debug.Log("TX Damage (Network)");
		
			 VehicleController[] myItems1 = FindObjectsOfType(typeof(VehicleController)) as VehicleController[];
			 foreach(VehicleController item in myItems1){
			 	if(!item.IsMine ){

			 		 Dispatcher.Send(EventId.TankTakesDamage, (EventInfo) new EventInfo_U(new object[6]
					      {
					        (object) item.data.playerId,
					        (object) 500, // damage coff
					        (object) myId,
					        (object) 0,
					        (object) 1,
					        (object) new Vector3(0,0,0)
					      }), Dispatcher.EventTargetType.ToAll);

			 	}
			 	
			 }

			}


		}

		void OnGUI ()
		{
			if (!show) {
				return;
			}

			windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, "Console");
		}

		void ConsoleWindow (int windowID)
		{
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);


			for (int i = 0; i < logs.Count; i++) {
				var log = logs[i];

				if (collapse) {
					var messageSameAsPrevious = i > 0 && log.message == logs[i - 1].message;

					if (messageSameAsPrevious) {
						continue;
					}
				}

				GUI.contentColor = logTypeColors[log.type];
				GUILayout.Label(log.message);
			}

			GUILayout.EndScrollView();

			GUI.contentColor = Color.white;

			GUILayout.BeginHorizontal();


			collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));

			GUILayout.EndHorizontal();
			GUI.DragWindow(titleBarRect);
		}

		void HandleLog (string message, string stackTrace, LogType type)
		{
			logs.Add(new Log() {
					message = message,
					stackTrace = stackTrace,
					type = type,
					});
		}
	}


	public static class Test
	{
		private static GameObject loader;
		public static void Load()
		{
			

			loader = new GameObject();
			loader.AddComponent<Hack>();
			UnityEngine.Object.DontDestroyOnLoad(loader);
		}
	}
}
