using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class LoginPageCreateController : LoginPageController
	{
		[SerializeField] private InputField UsernameInputField = default;
		[SerializeField] private InputField EmailInputField = default;
		[SerializeField] private InputField PasswordInputField = default;
		[SerializeField] private SimpleButton CreateButton = default;

		public static string LastUsername { get; private set; }
		public static string LastEmail { get; private set; }

		public static void DropLastCredentials()
		{
			LastUsername = null;
			LastEmail = null;
		}

		private bool IsCreateInProgress
		{
			get => base.IsInProgress;
			set
			{
				if (value == true)
				{
					base.OnStarted?.Invoke();
					Debug.Log("LoginPageCreateController: Create started");
				}
				else
					Debug.Log("LoginPageCreateController: Create ended");

				base.IsInProgress = value;
			}
		}

		private void Awake()
		{
			if (CreateButton != null)
				CreateButton.onClick += PrepareAndRunCreate;
		}

		private void Start()
		{
			if (!string.IsNullOrEmpty(LastUsername))
				UsernameInputField.text = LastUsername;

			if (!string.IsNullOrEmpty(LastEmail))
				EmailInputField.text = LastEmail;
		}

		private void PrepareAndRunCreate()
		{
			RunCreate(UsernameInputField.text, EmailInputField.text, PasswordInputField.text);
		}

		public void RunCreate(string username, string email, string password)
		{
			if (IsCreateInProgress)
				return;

			LastEmail = email;
			LastUsername = username;

			IsCreateInProgress = true;

			var isFieldsFilled = !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
			var isEmailValid = ValidateEmail(email);

			if (isFieldsFilled && isEmailValid)
			{
				Action onSuccessfulCreate = () =>
				{
					Debug.Log("LoginPageCreateController: Create success");
					base.OnSuccess?.Invoke();
				};

				Action<Error> onFailedCreate = error =>
				{
					Debug.LogError($"LoginPageCreateController: Create error: {error.ToString()}");
					base.OnError?.Invoke(error);
				};

				DemoController.Instance.LoginDemo.Registration(username, password, email, onSuccessfulCreate, onFailedCreate);
			}
			else if (!isEmailValid)
			{
				Debug.Log($"Invalid email: {email}");
				Error error = new Error(errorType: ErrorType.RegistrationNotAllowedException, errorMessage: "Invalid email");
				base.OnError?.Invoke(error);
			}
			else
			{
				Debug.LogError($"Fields are not filled. Username: '{username}' Password: '{password}'");
				Error error = new Error(errorType: ErrorType.RegistrationNotAllowedException, errorMessage: $"Not all fields are filled");
				base.OnError?.Invoke(error);
			}

			IsCreateInProgress = false;
		}
	}
}
