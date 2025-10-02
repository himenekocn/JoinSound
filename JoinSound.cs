
using System;
using Microsoft.Extensions.Configuration;
using Sharp.Shared;
using Sharp.Shared.Enums;
using Sharp.Shared.Listeners;
using Sharp.Shared.Managers;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;

namespace LynchMus.JoinSound;

public class JoinSound : IModSharpModule, IClientListener, IGameListener
{
    public string DisplayName => "JoinSound";
    public string DisplayAuthor => "LynchMus";

    private readonly ISharedSystem _shared;
    private readonly ISoundManager _soundManager;
    private readonly IModSharp _modSharp;

    public JoinSound(ISharedSystem sharedSystem,
        string? dllPath,
        string? sharpPath,
        Version? version,
        IConfiguration? coreConfiguration,
        bool hotReload)
    {
        ArgumentNullException.ThrowIfNull(dllPath);
        ArgumentNullException.ThrowIfNull(sharpPath);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(coreConfiguration);

        _shared = sharedSystem;
        _soundManager = _shared.GetSoundManager();
        _modSharp = _shared.GetModSharp();
    }

    public bool Init()
        => true;

    public void PostInit()
    {
        _shared.GetClientManager().InstallClientListener(this);
        _modSharp.InstallGameListener(this);
    }

    public void Shutdown()
    {
        _shared.GetClientManager().RemoveClientListener(this);
        _modSharp.RemoveGameListener(this);
    }

    #region ClientListener
    public void OnClientPostAdminCheck(IGameClient client)
    {
        if (client.IsValid && client is { IsHltv: false, IsFakeClient: false })
        {
            _soundManager.StartSoundEvent("Buttons.snd9", volume: 0.3f, filter: new RecipientFilter());
        }
    }

    public void OnClientDisconnected(IGameClient client, NetworkDisconnectionReason reason)
    {
        if (client.IsValid && client is { IsHltv: false, IsFakeClient: false })
        {
            _soundManager.StartSoundEvent("DoorHandles.Locked1", volume: 0.3f, filter: new RecipientFilter());
        }
    }

    int IClientListener.ListenerVersion => IClientListener.ApiVersion;
    int IClientListener.ListenerPriority => 0;
    #endregion

    #region GameListener
    public void OnResourcePrecache()
    {
        _modSharp.PrecacheResource("soundevents/game_sounds_world.vsndevts");
    }

    int IGameListener.ListenerVersion => IGameListener.ApiVersion;
    int IGameListener.ListenerPriority => 0;
    #endregion
}