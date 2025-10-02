
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

    private readonly ILogger<JoinSound> _logger;
    private readonly ISharedSystem _shared;
    public ISoundManager SoundManager { get; }
    public IModSharp ModSharp { get; }

    public JoinSound(ISharedSystem sharedSystem,
        string? dllPath,
        string? sharpPath,
        Version? version,
        IConfiguration? coreConfiguration,
        bool hotReload)
    {
        _logger = sharedSystem.GetLoggerFactory().CreateLogger<JoinSound>();
        _shared = sharedSystem;
        SoundManager = _shared.GetSoundManager();
        ModSharp = _shared.GetModSharp();
    }

    public bool Init()
        => true;

    public void PostInit()
    {
        _shared.GetClientManager().InstallClientListener(this);
        ModSharp.InstallGameListener(this);
    }

    public void Shutdown()
    {
        _shared.GetClientManager().RemoveClientListener(this);
        ModSharp.RemoveGameListener(this);
    }

    #region ClientListener
    public void OnClientPostAdminCheck(IGameClient client)
    {
        if (client.IsValid && !client.IsHltv && !client.IsFakeClient)
        {
            Playjoinsound();
        }
    }

    public void OnClientDisconnected(IGameClient client, NetworkDisconnectionReason reason)
    {
        if (client.IsValid && !client.IsHltv && !client.IsFakeClient)
        {
            Playleavesound();
        }
    }

    int IClientListener.ListenerVersion => IClientListener.ApiVersion;
    int IClientListener.ListenerPriority => 0;
    #endregion

    #region GameListener
    public void OnResourcePrecache()
    {
        ModSharp.PrecacheResource("soundevents/game_sounds_world.vsndevts");
    }

    int IGameListener.ListenerVersion => IGameListener.ApiVersion;
    int IGameListener.ListenerPriority => 0;
    #endregion


    public void Playjoinsound()
    {
        SoundManager.StartSoundEvent("Buttons.snd9", volume: 0.3f, filter: new RecipientFilter());
    }

    public void Playleavesound()
    {
        SoundManager.StartSoundEvent("DoorHandles.Locked1", volume: 0.3f, filter: new RecipientFilter());
    }
}