﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Dynamic;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace FivePD.API
{
    /* This class is referenced in the main plugin to call the CalloutAPI's `LoadCallouts` method */
    /// <summary>
    /// Implement this abstract class to make your own callout. This class defines the main callout structure as well as some basic helper methods.
    /// </summary>
    public abstract class Callout
    {
        /// <summary>
        /// The list of players that has accepted the callout
        /// NOTE: The base player is always going to be added by default
        /// </summary>
        public List<Ped> AssignedPlayers;
        /// <summary>
        /// The description of the callout that will be displayed in the computer (Detailed)
        /// </summary>
        public string CalloutDescription;
        /// <summary>
        /// The shortname of the callout that will be displayed on the notification's header (eg. Trespassing)
        /// </summary>
        public string ShortName;
        /// <summary>
        /// DON'T EDIT! CaseID will be automatically generated
        /// </summary>
        public string CaseID;
        /// <summary>
        /// The amount of XP that this call will add to the player's progression once completed<br/>
        /// (NOTE: Server owners can set a fix amount, which will override this XP)
        /// </summary>
        public uint XP;
        /// <summary>
        /// Response Code (eg. 1,2,3,...)
        /// </summary>
        public int ResponseCode;
        /// <summary>
        /// The distance that the distance checker will use to detemine whether the player is in range and start the callout 
        /// </summary>
        public float StartDistance;
        /// <summary>
        /// Location of the callout. This position will be marked on the map and set as a waypoint for the player
        /// </summary>
        public Vector3 Location;
        /// <summary>
        /// The blip will be created over the `Location`
        /// </summary>
        public Blip Marker;
        /// <summary>
        /// Do not set it! It'll be used as a unique identifier by CalloutManager when creating a call
        /// </summary>
        public string Identifier;
        /// <summary>
        /// It'll be set to true once the callout starts ( the player gets in range ).<br/>
        /// It is mainly used by the callout manager to determine whether to reward a player or not
        /// </summary>
        public bool Started
        {
            get;
            private set;
        }
        /// <summary>
        /// Marks whether the callout spawns at the same location (not random)
        /// If it is set to true, only 1 player can receive this call at the same time (meaning that it won't show up for others till the player completes the call)
        /// </summary>
        public bool FixedLocation
        {
            get;
            protected set;
        }

        public float Radius;
        public Callout() { }

        /* Gameplay related methods */

        /// <summary>
        /// Spawn a properly networked pedestrian that is also marked as mission entity. Since this method will also load in the model, this call must be awaited.
        /// </summary>
        /// <param name="pedHash">The <see cref="PedHash"/> that you want to spawn.</param>
        /// <param name="location">The location of the ped.</param>
        /// <param name="heading">Which direction the ped is facing.</param>
        /// <returns></returns>
        protected async Task<Ped> SpawnPed(PedHash pedHash, Vector3 location, float heading = 0f)
        {
            uint model = (uint)pedHash;

            RequestModel(model);

            while (!HasModelLoaded(model))
                await BaseScript.Delay(0);

            Ped ped = (Ped)Entity.FromHandle(CreatePed(0, model, location.X, location.Y, location.Z, heading, true, true));
            SetEntityAsMissionEntity(ped.Handle, true, true);

            return ped;
        }

        /// <summary>
        /// Spawn a properly networked vehicle that is also marked as mission entity. Since this method will also load in the model, this call must be awaited.
        /// </summary>
        /// <param name="vehicleHash">The <see cref="VehicleHash"/> of the vehicle that you want to spawn.</param>
        /// <param name="location">The location of the vehicle.</param>
        /// <param name="heading">Which direction the vehicle is facing.</param>
        /// <returns>The vehicle you spawned</returns>
        protected async Task<Vehicle> SpawnVehicle(VehicleHash vehicleHash, Vector3 location, float heading = 0f)
        {
            uint model = (uint)vehicleHash;

            RequestModel(model);

            while (!HasModelLoaded(model))
                await BaseScript.Delay(0);

            Vehicle vehicle = (Vehicle)Entity.FromHandle(CreateVehicle(model, location.X, location.Y, location.Z, heading, true, true));
            SetEntityAsMissionEntity(vehicle.Handle, true, true);
            
            return vehicle;
        }

        /// <summary>
        /// Returns a random PedHash, excluding animal Peds.
        /// </summary>
        private readonly PedHash[] hashes =
        {
            default,
            PedHash.Humpback,
            PedHash.Dolphin,
            PedHash.KillerWhale,
            PedHash.Fish,
            PedHash.HammerShark,
            PedHash.TigerShark,
            PedHash.Boar,
            PedHash.Cat,
            PedHash.ChickenHawk,
            PedHash.Chimp,
            PedHash.Coyote,
            PedHash.Cow,
            PedHash.Deer,
            PedHash.Pig,
            PedHash.Rabbit,
            PedHash.Crow,
            PedHash.Cormorant,
            PedHash.Husky,
            PedHash.Rottweiler,
            PedHash.Pug,
            PedHash.Poodle,
            PedHash.Retriever,
            PedHash.Seagull,
            PedHash.Pigeon,
            PedHash.MountainLion,
            PedHash.BradCadaverCutscene,
            PedHash.Chop,
            PedHash.Hen,
            PedHash.JohnnyKlebitz,
            PedHash.LamarDavisCutscene,
            PedHash.MagentaCutscene,
            PedHash.Marston01,
            PedHash.Misty01,
            PedHash.MovAlien01,
            PedHash.MoviePremFemaleCutscene,
            PedHash.MoviePremMaleCutscene,
            PedHash.MrsPhillipsCutscene,
            PedHash.MrKCutscene,
            PedHash.NataliaCutscene,
            PedHash.NigelCutscene,
            PedHash.NervousRonCutscene,
            PedHash.Niko01,
            PedHash.PaigeCutscene,
            PedHash.OscarCutscene,
            PedHash.OrtegaCutscene,
            PedHash.OrleansCutscene,
            PedHash.Orleans,
            PedHash.Pogo01,
            PedHash.Rat,
            PedHash.Rhesus,
            PedHash.Stingray,
            PedHash.SteveHainsCutscene,
            PedHash.Westy
        };

        /// <summary>
        /// Select a random <see cref="PedHash"/>.
        /// </summary>
        /// <returns></returns>
        protected PedHash GetRandomPed()
        {
            PedHash ped;
            var pedHashes = Enum.GetValues(typeof(PedHash));
            Random rnd = new Random();

            do
            {
                ped = (PedHash)pedHashes.GetValue(rnd.Next(0, pedHashes.Length));
            }
            while (Array.IndexOf(hashes, ped) != -1);

            return ped;
        }

        /// <summary>
        /// Initialize callout information. Call this in your callout constructor.
        /// </summary>
        /// <param name="location">The location for your callout.</param>
        protected void InitInfo(Vector3 location)
        {
            this.AssignedPlayers = new List<Ped>();
            this.AssignedPlayers.Add(Game.PlayerPed);
            this.CalloutDescription = "<Unnamed Callout Description>";
            this.ShortName = "<Unnamed Callout>";
            this.ResponseCode = -1;
            this.Location = location;
            this.Identifier = Guid.NewGuid().ToString();
            this.FixedLocation = false;
        }

        /// <summary>
        /// OnAccept will be called when the player accepts the call.
        /// You must call base.OnAccept(args) to initialise the default properties
        /// </summary>
        protected void InitBlip(float circleRadius = 75f, BlipColor color = BlipColor.Yellow, BlipSprite sprite = BlipSprite.BigCircle, int alpha = 100)
        {
            int blipHandle = AddBlipForRadius(this.Location.X, this.Location.Y, this.Location.Z, circleRadius);
            this.Radius = circleRadius;
            this.Marker = new Blip(blipHandle);
            this.Marker.Sprite = sprite;
            this.Marker.Color = color;
            this.Marker.Alpha = alpha;
        }

        /// <summary>
        /// OnAccept() will be automatically invoked by the CalloutManager<br/>
        /// Define game logic here (eg. Spawn suspects,victims,vehicles)
        /// </summary>
        public virtual async Task OnAccept() { }

        /// <summary>
        /// (Do not call it)<br/><br/>
        /// Destructs every spawned object automatically if defined as a field or property (can be public, private, protected and static)<br/>
        /// To spawn something, define it as a property or field, otherwise you'll manually have to delete it locally.<br />
        /// See the documentation.
        /// </summary>
        #region Events
        public virtual void OnStart(Ped closest)
        {
            this.Started = true;
            if (closest.NetworkId != Game.PlayerPed.NetworkId)
            {
                this.AssignedPlayers.Add(closest);
            }
        }

        /// <summary>
        /// Called when backup is requested through the Callout menu.
        /// </summary>
        /// <param name="code">The code of the backup. Either 1, 2, 3 or 99.</param>
        public virtual void OnBackupCalled(int code) { }

        /// <summary>
        /// Called when a player has accepted the backup request and is added to the call.
        /// </summary>
        /// <param name="player">The player that was added to the call.</param>
        public virtual void OnBackupReceived(Player player) { }

        /// <summary>
        /// Called when a player that was on the call has canceled the assistance (left the call).
        /// </summary>
        /// <param name="player">The player that has left the call.</param>
        public virtual void OnPlayerRevokedBackup(Player player) { }

        /// <summary>
        /// Called before the callout is cleaned up (before Destruct()).
        /// </summary>
        public virtual void OnCancelBefore() { }

        /// <summary>
        /// Called after the callout is cleaned up.
        /// </summary>
        public virtual void OnCancelAfter() { }

        /// <summary>
        /// EndCallout() should be called when you want to terminate or end the callout.<br /><br />
        /// You should have conditions with your logic when you want to end your callout (Eg. when the player goes to a certain point on the map, ...)<br />
        /// Calling this method will automatically mark it as a completed callout<br />
        /// If you don't call this method, the user will have to manually cancel the callout.<br /><br />
        /// Called methods after calling EndCallout() method:<br />
        /// - OnCancelBefore()<br />
        /// - OnCancelAfter()
        /// </summary>
        public Action EndCallout { get; set; }

        public delegate void ShowDialogDelegate(string text, int duration ,float showRadius);
        public ShowDialogDelegate ShowDialog { get; set; }

        public delegate void ShowNotificationDelegate(string text, string textureDict, string textureName, string sender, string subject, float showRadius);
        public ShowNotificationDelegate ShowNetworkedNotification { get; set; }

        public delegate void UpdateDataDelegate(string location = null);
        public UpdateDataDelegate UpdateData { get; set; }

        /// <summary>
        /// This method allows you to influence whether this callout is currently allowed to initialize or not.
        /// Return <c>true</c> to enable the callout, <c>false</c> to disable.<br /><br />
        /// <example>
        /// In the following example, the callout will only appear during the night (from midnight to 6AM).
        /// <code>
        /// public override Task&lt;bool&gt; CheckRequirements() => Task.FromResult(World.CurrentDateTime &lt;= TimeSpan.FromHours(6));
        /// </code>
        /// </example>
        /// </summary>
        /// <returns>True to enable, false to disable.</returns>
        public virtual async Task<bool> CheckRequirements()
        {
            return true;
        }

        /* Experimental below */
        public List<object> Clues;
        /* If a criminal gets X distance away, attach a question mark nearby at every Y secs */
        protected void AttachClueToPed(Ped ped, float minDistance, int repeat = 15)
        {
            if (this.Clues == null)
                this.Clues = new List<object>();

            var clue = new
            {
                PedNetworkID = ped.NetworkId,
                minDistance,
                repeat
            };
            this.Clues.Add(clue);
        }

        /// <summary>
        /// Receive Tick from the callout manager.
        /// To subscribe to ticks, please use the <see cref="Tick"/> event.
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="Tick"/>
        public async Task ReceiveTick()
        {
            if (Tick != null)
                await Tick.Invoke();
        }

        /// <summary>
        /// Subscribe to the tick event to process callout logic.
        /// <example>
        /// In order to subscribe to the event, register an event handler:
        /// <code>
        /// public override void OnStart(Ped closest)
        /// {
        ///     Tick += OnTick;
        /// }
        /// </code>
        /// Then, define your event handler. In this event handler you can write your logic.
        /// <code>
        /// public async Task OnTick()
        /// {
        ///     Debug.WriteLine("A tick!");
        ///     
        ///     await BaseScript.Delay(1500);
        /// }
        /// </code>
        /// In this example you would see "A tick!" appearing in the console (F8) every 1,5 seconds. Make sure to call BaseScript.Delay!
        /// </example>
        /// </summary>
        protected event Func<Task> Tick;

        #endregion
    }

    /// <summary>
    /// Bundles callout properties (CalloutPropertiesAttribute is used in the CalloutManager)
    /// </summary>
    public class CalloutPropertiesAttribute : Attribute
    {
        /// <summary>The name of the callout as shown in the Debug menu and in the console (See <see cref="Callout.ShortName"/> for the dispatch name)</summary>
        public string name { get; private set; }
        public string author { get; private set; }

        /// <summary>
        /// The version of the callout.
        /// <example>
        /// For example: <c>1.2.3</c>
        /// </example>
        /// </summary>
        public string version { get; private set; }

        /// <summary>
        /// Callout properties
        /// </summary>
        /// <param name="name">The name of the callout as shown in the Debug menu and in the console (See <see cref="Callout.ShortName"/> for the dispatch name)</param>
        /// <param name="version">The version of the callout. <example>For example: <c>1.2.3</c></example></param>
        public CalloutPropertiesAttribute(string name, string author, string version)
        {
            this.name = name;
            this.version = version;
            this.author = author;
        }
    }
}