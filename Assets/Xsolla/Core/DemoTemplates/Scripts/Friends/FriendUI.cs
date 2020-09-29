﻿using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

public class FriendUI : MonoBehaviour
{
    [SerializeField] private Image avatarImage;
    [SerializeField] private Image statusImage;
    [SerializeField] private Text nickname;
    [SerializeField] private FriendActionsButton actionsButton;
    [SerializeField] private FriendButtonsUI userButtons;
    [SerializeField] private FriendStatusLineUI userStatusLine;
    
    public FriendModel FriendModel { get; private set; }
    
    public void Initialize(FriendModel friend)
    {
        if(friend == null) return;
        FriendModel = friend;

        InitAvatar(FriendModel);

        InitStatus(FriendModel);

        InitNickname(FriendModel);
        
        SetUserState(FriendModel.Relationship);
    }

    private void InitAvatar(FriendModel friend)
    {
        if (!string.IsNullOrEmpty(friend.AvatarUrl) && avatarImage != null)
        {
            if (!string.IsNullOrEmpty(friend.AvatarUrl))
            {
                ImageLoader.Instance.GetImageAsync(friend.AvatarUrl, (url, sprite) =>
                {
                    avatarImage.gameObject.SetActive(true);
                    avatarImage.sprite = sprite;
                });    
            }
            else
            {
                Debug.LogError($"Friend with nickname = '{friend.Nickname}' without image!");
            }
        }
    }

    private void InitStatus(FriendModel friend)
    {
        if (statusImage != null)
        {
            statusImage.gameObject.SetActive(true);
            switch (friend.Status)
            {
                case UserOnlineStatus.Online:
                    statusImage.color = new Color(0.043F, 0.043F, 0, 1.0F);
                    break;
                case UserOnlineStatus.Offline:
                    statusImage.color = new Color(0.517F, 0.557F, 0.694F, 0.5F);
                    break;
                case UserOnlineStatus.Unknown:
                    statusImage.color = new Color(0.5F, 0.5F, 0.5F, 0.5F);
                    break;
                default:
                    statusImage.color = new Color(0.8F, 0F, 0F, 0.3F);
                    break;
            }
        }
    }

    private void InitNickname(FriendModel friend)
    {
        var text = string.IsNullOrEmpty(friend.Nickname) ? string.Empty : friend.Nickname;
        if(!string.IsNullOrEmpty(text))
            gameObject.name = text;
        if (nickname != null)
            nickname.text = text;
    }

    public void SetUserState(UserState state)
    {
        BaseUserStateUI userState;
        switch (state)
        {
            case UserState.Initial:
            {
                userState = gameObject.AddComponent<UserStateInitial>();
                break;
            }
            case UserState.MyFriend:
            {
                userState = gameObject.AddComponent<UserStateMyFriend>();
                break;
            }
            case UserState.Pending:
            {
                userState = gameObject.AddComponent<UserStatePending>();
                break;
            }
            case UserState.Requested:
            {
                userState = gameObject.AddComponent<UserStateRequested>();
                break;
            }
            case UserState.Blocked:
            {
                userState = gameObject.AddComponent<UserStateBlocked>();
                break;
            }
            default:
            {
                Debug.LogWarning($"Set up handle of user state = '{state.ToString()}' in FriendUI.cs");
                return;
            }
        }
        userState.Init(this, userButtons, userStatusLine, actionsButton);
    }

    public void SetUserState(UserRelationship relationship)
    {
        switch (relationship)
        {
            case UserRelationship.Unknown:
            {
                SetUserState(UserState.Initial);
                break;
            }
            case UserRelationship.Friend:
            {
                SetUserState(UserState.MyFriend);
                break;
            }
            case UserRelationship.Pending:
            {
                SetUserState(UserState.Pending);
                break;
            }
            case UserRelationship.Requested:
            {
                SetUserState(UserState.Requested);
                break;
            }
            case UserRelationship.Blocked:
            {
                SetUserState(UserState.Blocked);
                break;
            }
            default:
            {
                Debug.LogException(new ArgumentOutOfRangeException(nameof(relationship), relationship, null));
                break;
            }
        }
    }
}
