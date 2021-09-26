using MVC.View;
using Net.Client;
using Net.Component;
using Net.Share;
using UnityEngine;
using UnityEngine.UI;

namespace Hotfix
{
	//�ȸ������ɵĽű�, �뿴gitee��mvcģ��ʹ�ý���ͼʾ
	public class LoginPanel
	{
		public static LoginPanel Instance = new LoginPanel();
		public GameObject panel;
		public InputField pwd;
		public InputField acc;
		public Button login;
		public Button signUp;
		public Button Btn_Close;

		public void Init(FieldCollection fc)
		{
			panel = fc.gameObject;
			pwd = fc["pwd"].target as InputField;
			acc = fc["acc"].target as InputField;
			login = fc["login"].target as Button;
			signUp = fc["signUp"].target as Button;
			Btn_Close = fc["Btn_Close"].target as Button;
			login.onClick.AddListener(() => {
				if (acc.text.Length <= 0 | pwd.text.Length <= 0)
				{
					MsgPanel.Show("��¼����!");
					return;
				}
				ClientManager.Instance.SendRT("Login", acc.text, pwd.text);
			});
			signUp.onClick.AddListener(() => {
				RegisterPanel.Show();
			});
			Btn_Close.onClick.AddListener(() => {
				
			});
			ClientManager.Instance.client.Add_ILR_RpcHandle(this);
		}

		internal static void Show() 
		{
			Instance.panel.SetActive(true);
		}

        internal static void Hide()
        {
			Instance.panel.SetActive(false);
		}

		[Rpc]
		void LoginCallback(bool result, string info)
		{
			if (result)
			{
				Hide();
				UnityEngine.SceneManagement.SceneManager.LoadScene(1);
			}
			else MsgPanel.Show(info);
		}
	}
}