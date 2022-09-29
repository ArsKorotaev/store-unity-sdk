using UnityEditor;
using UnityEngine;

namespace Xsolla.Core
{
	[CustomEditor(typeof(XsollaSettings))]
	public partial class XsollaSettingsEditor : UnityEditor.Editor
	{
		[MenuItem("Window/Xsolla/Edit Settings", false, 1000)]
		public static void Edit()
		{
			Selection.activeObject = XsollaSettings.Instance;
		}

		private GUIStyle GroupAreaStyle => GUI.skin.box;
		
		private GUIStyle GroupHeaderStyle => EditorStyles.boldLabel;

		public override void OnInspectorGUI()
		{
			var changed = GeneralSettings() ||
			              LoginSettings() ||
			              PayStationSettings() ||
			              AndroidSettings() ||
			              DesktopSettings() ||
			              EditorSettings();

			if (changed)
				DropSavedTokens();
		}

		public static void DropSavedTokens()
		{
			PlayerPrefs.DeleteKey(Constants.LAST_SUCCESS_AUTH_TOKEN);
			PlayerPrefs.DeleteKey(Constants.LAST_SUCCESS_OAUTH_REFRESH_TOKEN);
		}
	}
}