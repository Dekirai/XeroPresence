using DiscordRPC;
using XeroPresence.Properties;
using System.Diagnostics;
using Newtonsoft.Json;
using File = System.IO.File;
using IWshRuntimeLibrary;
using System.Text;

namespace XeroPresence
{
    public partial class MainForm : Form
    {
        private static DiscordRpcClient discord;
        private Dictionary<string, Dictionary<string, string>> iniData;
        public static bool _discordLoggedIn = false;

        public static int _pen = 0;
        public static int _zp = 0;
        public static int _gems = 0;
        public static string _nickname = "";
        public static string _clan = "";
        public static string _clanimage = "";
        public static int _level = 0;
        public static string _levelimage = "";
        public static int _xp = 0;
        public static int _xprequired = 0;
        public static int _xppercentage = 0;
        public static int _xptotal = 0;
        public static string _channel = "";
        public static int _id = 0;
        public static string _name = "";
        public static int _scorelimit = 0;
        public static int _playerlimit = 0;
        public static int _playercount = 0;
        public static string _mode = "";
        public static string _map = "";
        public static string _mapimage = "";
        public static int _scoreAlpha = 0;
        public static int _scoreBeta = 0;
        public static string _team = "";
        public static string _status = "";
        public static int _ping = 0;
        public static int _totalScore = 0;
        public static int _score = 0;
        public static int _kills = 0;
        public static int _goals = 0;
        public static int _deaths = 0;
        public static string _gameTimeState = "";
        public static string _gameState = "";
        public static int _wins = 0;
        public static int _chaserCount = 0;
        public static int _survived = 0;

        public static bool _overwritedetails = false;
        public static bool _overwritestate = false;
        public static bool _overwritelargeasset = false;
        public static bool _overwritesmallasset = false;

        public static string _details = "";
        public static string _state = "";
        public static string _largeasseturl = "";
        public static string _largeassettext = "";
        public static string _smallasseturl = "";
        public static string _smallassettext = "";

        public static string _newdetails = "";
        public static string _newstate = "";
        public static string _newlargeasseturl = "";
        public static string _newlargeassettext = "";
        public static string _newsmallasseturl = "";
        public static string _newsmallassettext = "";

        public MainForm()
        {
            InitializeComponent();
            iniData = new Dictionary<string, Dictionary<string, string>>();
            LoadIniFile("profiles.ini");
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
            _discordLoggedIn = discord.IsInitialized;
        }

        private async void button1_Click(object sender, EventArgs e)
        {

            bt_login.Text = "Logging in...";
            timer1.Start();
            cb_Profiles.Enabled = false;
            tb_accesskey.ReadOnly = true;
            tb_accesskeysecret.ReadOnly = true;
            bt_login.Enabled = false;
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                string customtext = "";
                Process[] xero = Process.GetProcessesByName("xeroclient");

                //We do not want to send requests to the API if the game isn't even running
                if (xero.Length == 0)
                {
                    bt_login.Text = "Waiting for Xero...";
                    if (_discordLoggedIn == true)
                    {
                        try
                        {
                            discord.Deinitialize();
                            _discordLoggedIn = false;
                            return;
                        }
                        catch
                        {
                            _discordLoggedIn = false;
                            return;
                        }
                    }
                }
                else
                {

                    if (_discordLoggedIn == false)
                    {
                        discord = new DiscordRpcClient("1092449168703901756");
                        InitializeDiscord();
                    }

                    var client = new HttpClient();

                    client.DefaultRequestHeaders.Add("x-api-access-key-id", tb_accesskey.Text);
                    client.DefaultRequestHeaders.Add("x-api-secret-access-key", tb_accesskeysecret.Text);

                    var response = await client.GetAsync($"https://xero.gg/api/self/status/v?time={DateTime.Now}");
                    var responseContent = await response.Content.ReadAsStringAsync();

                    dynamic jsonData = JsonConvert.DeserializeObject(responseContent);

                    string assemblyPath = Process.GetCurrentProcess().MainModule.FileName;
                    string exePath = Path.GetDirectoryName(assemblyPath);

                    bool _success = jsonData.success;
                    if (_success == false)
                    {
                        string _reason = jsonData.text;
                        if (_reason == "Please wait a moment.")
                            return;
                        MessageBox.Show($"Error while trying to read the API.\nReason: {_reason}\n\nPlease click on 'Start Presence' again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        bt_login.Enabled = true;
                        bt_login.Text = "Start Presence";
                        this.Visible = true;
                        this.ShowInTaskbar = true;
                        notifyIcon.Visible = false;
                        cb_HideInTray.Checked = false;
                        try
                        {
                            _discordLoggedIn = false;
                            discord.Deinitialize();
                            timer1.Stop();
                            return;
                        }
                        catch
                        {
                            timer1.Stop();
                            return;
                        }
                    }

                    bt_login.Text = "Logged in";
                    cb_Profiles.Enabled = true;

                    var _isOnline = jsonData.game.online;
                    var _isServer = jsonData.game.server;
                    var _isChannel = jsonData.game.channel;

                    if (_isOnline == null || _isServer == null || _isChannel == null)
                        return;

                    var _clancheck = jsonData.info.clan;
                    if (_clancheck != null)
                    {
                        _clan = jsonData.info.clan.name;
                        _clanimage = jsonData.info.clan.image;
                    }
                    _channel = jsonData.game.channel.name;
                    _nickname = jsonData.info.name;
                    _level = jsonData.info.progression.level.value;
                    _levelimage = jsonData.info.progression.level.image;
                    _xp = jsonData.info.progression.level.progress.current;
                    _xprequired = jsonData.info.progression.level.progress.required;
                    _xppercentage = jsonData.info.progression.level.progress.percentage;
                    _xptotal = jsonData.info.progression.xp;
                    _pen = jsonData.currency.pen;
                    _zp = jsonData.currency.zp;
                    _gems = jsonData.currency.gems;
                    var _roomstate = jsonData.game.room;

                    if (_roomstate != null)
                    {
                        var _playerdata = jsonData.game.room.match.playerData;
                        _id = jsonData.game.room.id;
                        _name = jsonData.game.room.name;
                        int _timelimit = jsonData.game.room.timeLimit;
                        _scorelimit = jsonData.game.room.scoreLimit;
                        bool _isfriendly = jsonData.game.room.isFriendly;
                        bool _isPasswordProtected = jsonData.game.room.isPasswordProtected;
                        _playerlimit = jsonData.game.room.playerLimit;
                        _playercount = jsonData.game.room.playerCount;
                        _mode = jsonData.game.room.mode.name;
                        _map = jsonData.game.room.map.name;
                        _mapimage = jsonData.game.room.map.image;
                        _ping = jsonData.game.room.match.playerData.ping;
                        _team = jsonData.game.room.match.playerData.team.name;

                        if (_playerdata.isIngame != false)
                        {
                            _gameState = jsonData.game.room.match.gameState.name;
                            _gameTimeState = jsonData.game.room.match.gameTimeState.name;
                            int _gameTime = jsonData.game.room.match.gameTime;
                            int _roundTime = jsonData.game.room.match.roundTime;
                            _status = jsonData.game.room.match.playerData.state.name;

                            var _minutes = _gameTime / 60;
                            var _seconds = _gameTime % 60;
                            var _maxtimeminutes = _timelimit / 60;
                            var _maxtimeseconds = _timelimit % 60;
                            discord.UpdateLargeAsset(_mapimage, $"Playing on {_map} ({_mode})");
                            discord.UpdateDetails($"{_nickname} » {_channel} » #{_id}");

                            if (_mode == "Touchdown")
                            {
                                _scoreAlpha = jsonData.game.room.match.modeData.score.alpha;
                                _scoreBeta = jsonData.game.room.match.modeData.score.beta;
                                _totalScore = jsonData.game.room.match.playerData.record.totalScore;
                                _kills = jsonData.game.room.match.playerData.record.kills;
                                _goals = jsonData.game.room.match.playerData.record.goalsScore;
                                _deaths = jsonData.game.room.match.playerData.record.deaths;

                                if (File.Exists(exePath + "/config.json"))
                                {
                                    customtext = File.ReadAllText(exePath + "/config.json");
                                    dynamic _customtext = JsonConvert.DeserializeObject(customtext);
                                    _overwritedetails = _customtext.Touchdown.OverwriteDetails;
                                    _overwritestate = _customtext.Touchdown.OverwriteState;
                                    _overwritelargeasset = _customtext.Touchdown.OverwriteLargeAsset;
                                    _overwritesmallasset = _customtext.Touchdown.OverwriteSmallAsset;

                                    _details = _customtext.Touchdown.Details;
                                    _state = _customtext.Touchdown.State;
                                    _largeasseturl = _customtext.Touchdown.LargeAssetURL;
                                    _largeassettext = _customtext.Touchdown.LargeAssetText;
                                    _smallasseturl = _customtext.Touchdown.SmallAssetURL;
                                    _smallassettext = _customtext.Touchdown.SmallAssetText;

                                    _newdetails = ReplaceTags.TD(_details);
                                    _newstate = ReplaceTags.TD(_state);
                                    _newlargeasseturl = ReplaceTags.TD(_largeasseturl);
                                    _newlargeassettext = ReplaceTags.TD(_largeassettext);
                                    _newsmallasseturl = ReplaceTags.TD(_smallasseturl);
                                    _newsmallassettext = ReplaceTags.TD(_smallassettext);
                                }
                                else
                                    ResetConfig();

                                if (_overwritesmallasset)
                                    discord.UpdateSmallAsset(_newsmallasseturl, _newsmallassettext);
                                else
                                {
                                    if (_isPasswordProtected)
                                        discord.UpdateSmallAsset("lock", "This room is password protected");
                                    else
                                        discord.UpdateSmallAsset("", "");
                                }
                                if (_overwritelargeasset)
                                    discord.UpdateLargeAsset(_newlargeasseturl, _newlargeassettext);
                                else
                                    discord.UpdateLargeAsset(_mapimage, $"Playing on {_map} ({_mode})");
                                if (_overwritedetails)
                                    discord.UpdateDetails(_newdetails);
                                else
                                    discord.UpdateDetails($"{_nickname} » {_channel} » #{_id}");
                                if (_overwritestate)
                                    discord.UpdateState(_newstate);
                                else
                                    discord.UpdateState($"{_name} | {_gameTimeState} | {_scoreAlpha} - {_scoreBeta}");

                                int remainingSeconds = (_timelimit / 2) - _roundTime;
                                if (_gameTimeState == "FirstHalf" || _gameTimeState == "SecondHalf")
                                {
                                    discord.UpdateStartTime(DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(_roundTime)));
                                    discord.UpdateEndTime(DateTime.UtcNow.AddSeconds(remainingSeconds));
                                }
                                else
                                    discord.UpdateClearTime();
                            }
                            else if (_mode == "Deathmatch")
                            {
                                _scoreAlpha = jsonData.game.room.match.modeData.score.alpha;
                                _scoreBeta = jsonData.game.room.match.modeData.score.beta;
                                _totalScore = jsonData.game.room.match.playerData.record.totalScore;
                                _kills = jsonData.game.room.match.playerData.record.kills;
                                _deaths = jsonData.game.room.match.playerData.record.deaths;

                                if (File.Exists(exePath + "/config.json"))
                                {
                                    customtext = File.ReadAllText(exePath + "/config.json");
                                    dynamic _customtext = JsonConvert.DeserializeObject(customtext);
                                    _overwritedetails = _customtext.Deathmatch.OverwriteDetails;
                                    _overwritestate = _customtext.Deathmatch.OverwriteState;
                                    _overwritelargeasset = _customtext.Deathmatch.OverwriteLargeAsset;
                                    _overwritesmallasset = _customtext.Deathmatch.OverwriteSmallAsset;

                                    _details = _customtext.Deathmatch.Details;
                                    _state = _customtext.Deathmatch.State;
                                    _largeasseturl = _customtext.Deathmatch.LargeAssetURL;
                                    _largeassettext = _customtext.Deathmatch.LargeAssetText;
                                    _smallasseturl = _customtext.Deathmatch.SmallAssetURL;
                                    _smallassettext = _customtext.Deathmatch.SmallAssetText;

                                    _newdetails = ReplaceTags.DM(_details);
                                    _newstate = ReplaceTags.DM(_state);
                                    _newlargeasseturl = ReplaceTags.DM(_largeasseturl);
                                    _newlargeassettext = ReplaceTags.DM(_largeassettext);
                                    _newsmallasseturl = ReplaceTags.DM(_smallasseturl);
                                    _newsmallassettext = ReplaceTags.DM(_smallassettext);
                                }
                                else
                                    ResetConfig();

                                if (_overwritesmallasset)
                                    discord.UpdateSmallAsset(_newsmallasseturl, _newsmallassettext);
                                else
                                {
                                    if (_isPasswordProtected)
                                        discord.UpdateSmallAsset("lock", "This room is password protected");
                                    else
                                        discord.UpdateSmallAsset("", "");
                                }
                                if (_overwritelargeasset)
                                    discord.UpdateLargeAsset(_newlargeasseturl, _newlargeassettext);
                                else
                                    discord.UpdateLargeAsset(_mapimage, $"Playing on {_map} ({_mode})");
                                if (_overwritedetails)
                                    discord.UpdateDetails(_newdetails);
                                else
                                    discord.UpdateDetails($"{_nickname} » {_channel} » #{_id}");
                                if (_overwritestate)
                                    discord.UpdateState(_newstate);
                                else
                                    discord.UpdateState($"{_name} | {_gameTimeState} | {_scoreAlpha} - {_scoreBeta}");

                                int remainingSeconds = (_timelimit / 2) - _roundTime;
                                if (_gameTimeState == "FirstHalf" || _gameTimeState == "SecondHalf")
                                {
                                    discord.UpdateStartTime(DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(_roundTime)));
                                    discord.UpdateEndTime(DateTime.UtcNow.AddSeconds(remainingSeconds));
                                }
                                else
                                    discord.UpdateClearTime();
                            }
                            else if (_mode == "Battle Royal")
                            {
                                _totalScore = jsonData.game.room.match.playerData.record.totalScore;
                                _kills = jsonData.game.room.match.playerData.record.kills;
                                _deaths = jsonData.game.room.match.playerData.record.deaths;

                                if (File.Exists(exePath + "/config.json"))
                                {
                                    customtext = File.ReadAllText(exePath + "/config.json");
                                    dynamic _customtext = JsonConvert.DeserializeObject(customtext);
                                    _overwritedetails = _customtext.BattleRoyal.OverwriteDetails;
                                    _overwritestate = _customtext.BattleRoyal.OverwriteState;
                                    _overwritelargeasset = _customtext.BattleRoyal.OverwriteLargeAsset;
                                    _overwritesmallasset = _customtext.BattleRoyal.OverwriteSmallAsset;

                                    _details = _customtext.BattleRoyal.Details;
                                    _state = _customtext.BattleRoyal.State;
                                    _largeasseturl = _customtext.BattleRoyal.LargeAssetURL;
                                    _largeassettext = _customtext.BattleRoyal.LargeAssetText;
                                    _smallasseturl = _customtext.BattleRoyal.SmallAssetURL;
                                    _smallassettext = _customtext.BattleRoyal.SmallAssetText;

                                    _newdetails = ReplaceTags.BR(_details);
                                    _newstate = ReplaceTags.BR(_state);
                                    _newlargeasseturl = ReplaceTags.BR(_largeasseturl);
                                    _newlargeassettext = ReplaceTags.BR(_largeassettext);
                                    _newsmallasseturl = ReplaceTags.BR(_smallasseturl);
                                    _newsmallassettext = ReplaceTags.BR(_smallassettext);
                                }
                                else
                                    ResetConfig();

                                if (_overwritesmallasset)
                                    discord.UpdateSmallAsset(_newsmallasseturl, _newsmallassettext);
                                else
                                {
                                    if (_isPasswordProtected)
                                        discord.UpdateSmallAsset("lock", "This room is password protected");
                                    else
                                        discord.UpdateSmallAsset("", "");
                                }
                                if (_overwritelargeasset)
                                    discord.UpdateLargeAsset(_newlargeasseturl, _newlargeassettext);
                                else
                                    discord.UpdateLargeAsset(_mapimage, $"Playing on {_map} ({_mode})");
                                if (_overwritedetails)
                                    discord.UpdateDetails(_newdetails);
                                else
                                    discord.UpdateDetails($"{_nickname} » {_channel} » #{_id}");
                                if (_overwritestate)
                                    discord.UpdateState(_newstate);
                                else
                                    discord.UpdateState($"{_name} | {_gameState}");

                                int remainingSeconds = _timelimit - _roundTime;
                                discord.UpdateStartTime(DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(_roundTime)));
                                discord.UpdateEndTime(DateTime.UtcNow.AddSeconds(remainingSeconds));
                            }
                            else if (_mode == "Chaser")
                            {
                                _totalScore = jsonData.game.room.match.playerData.record.totalScore;
                                _kills = jsonData.game.room.match.playerData.record.kills;
                                _deaths = jsonData.game.room.match.playerData.record.deaths;
                                _chaserCount = jsonData.game.room.match.playerData.record.chaserCount;
                                _survived = jsonData.game.room.match.playerData.record.survived;
                                _wins = jsonData.game.room.match.playerData.record.wins;

                                if (File.Exists(exePath + "/config.json"))
                                {
                                    customtext = File.ReadAllText(exePath + "/config.json");
                                    dynamic _customtext = JsonConvert.DeserializeObject(customtext);
                                    _overwritedetails = _customtext.Chaser.OverwriteDetails;
                                    _overwritestate = _customtext.Chaser.OverwriteState;
                                    _overwritelargeasset = _customtext.Chaser.OverwriteLargeAsset;
                                    _overwritesmallasset = _customtext.Chaser.OverwriteSmallAsset;

                                    _details = _customtext.Chaser.Details;
                                    _state = _customtext.Chaser.State;
                                    _largeasseturl = _customtext.Chaser.LargeAssetURL;
                                    _largeassettext = _customtext.Chaser.LargeAssetText;
                                    _smallasseturl = _customtext.Chaser.SmallAssetURL;
                                    _smallassettext = _customtext.Chaser.SmallAssetText;

                                    _newdetails = ReplaceTags.CH(_details);
                                    _newstate = ReplaceTags.CH(_state);
                                    _newlargeasseturl = ReplaceTags.CH(_largeasseturl);
                                    _newlargeassettext = ReplaceTags.CH(_largeassettext);
                                    _newsmallasseturl = ReplaceTags.CH(_smallasseturl);
                                    _newsmallassettext = ReplaceTags.CH(_smallassettext);
                                }
                                else
                                    ResetConfig();

                                if (_overwritesmallasset)
                                    discord.UpdateSmallAsset(_newsmallasseturl, _newsmallassettext);
                                else
                                {
                                    if (_isPasswordProtected)
                                        discord.UpdateSmallAsset("lock", "This room is password protected");
                                    else
                                        discord.UpdateSmallAsset("", "");
                                }
                                if (_overwritelargeasset)
                                    discord.UpdateLargeAsset(_newlargeasseturl, _newlargeassettext);
                                else
                                    discord.UpdateLargeAsset(_mapimage, $"Playing on {_map} ({_mode})");
                                if (_overwritedetails)
                                    discord.UpdateDetails(_newdetails);
                                else
                                    discord.UpdateDetails($"{_nickname} » {_channel} » #{_id}");
                                if (_overwritestate)
                                    discord.UpdateState(_newstate);
                                else
                                    discord.UpdateState($"{_name} | {_gameState}");

                                int remainingSeconds = _timelimit - _roundTime;
                                discord.UpdateStartTime(DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(_roundTime)));
                                discord.UpdateEndTime(DateTime.UtcNow.AddSeconds(remainingSeconds));
                            }

                            if (_isPasswordProtected)
                                discord.UpdateSmallAsset("lock", "This room is password protected");
                            else
                                discord.UpdateSmallAsset("", "");

                            DiscordRPC.Button[] buttons = new DiscordRPC.Button[1];
                            buttons[0] = new DiscordRPC.Button { Label = "View Profile", Url = $"https://xero.gg/player/{_nickname}" };

                            discord.UpdateButtons(buttons);
                        }
                        else
                        {
                            if (File.Exists(exePath + "/config.json"))
                            {
                                customtext = File.ReadAllText(exePath + "/config.json");
                                dynamic _customtext = JsonConvert.DeserializeObject(customtext);
                                _overwritedetails = _customtext.Room.OverwriteDetails;
                                _overwritestate = _customtext.Room.OverwriteState;
                                _overwritelargeasset = _customtext.Room.OverwriteLargeAsset;
                                _overwritesmallasset = _customtext.Room.OverwriteSmallAsset;

                                _details = _customtext.Room.Details;
                                _state = _customtext.Room.State;
                                _largeasseturl = _customtext.Room.LargeAssetURL;
                                _largeassettext = _customtext.Room.LargeAssetText;
                                _smallasseturl = _customtext.Room.SmallAssetURL;
                                _smallassettext = _customtext.Room.SmallAssetText;

                                _newdetails = ReplaceTags.Room(_details);
                                _newstate = ReplaceTags.Room(_state);
                                _newlargeasseturl = ReplaceTags.Room(_largeasseturl);
                                _newlargeassettext = ReplaceTags.Room(_largeassettext);
                                _newsmallasseturl = ReplaceTags.Room(_smallasseturl);
                                _newsmallassettext = ReplaceTags.Room(_smallassettext);
                            }
                            else
                                ResetConfig();

                            if (_overwritesmallasset)
                                discord.UpdateSmallAsset(_newsmallasseturl, _newsmallassettext);
                            else
                            {
                                if (_isPasswordProtected)
                                    discord.UpdateSmallAsset("lock", "This room is password protected");
                                else
                                    discord.UpdateSmallAsset("", "");
                            }
                            if (_overwritelargeasset)
                                discord.UpdateLargeAsset(_newlargeasseturl, _newlargeassettext);
                            else
                                discord.UpdateLargeAsset(_mapimage, $"Playing on {_map} ({_mode})");
                            if (_overwritedetails)
                                discord.UpdateDetails(_newdetails);
                            else
                                discord.UpdateDetails($"{_nickname} » {_channel} » #{_id}");
                            if (_overwritestate)
                                discord.UpdateState(_newstate);
                            else
                                discord.UpdateState($"{_name} | Waiting");
                            DiscordRPC.Button[] buttons = new DiscordRPC.Button[1];
                            buttons[0] = new DiscordRPC.Button { Label = "View Profile", Url = $"https://xero.gg/player/{_nickname}" };
                            discord.UpdateClearTime();
                            discord.UpdateButtons(buttons);
                        }
                    }
                    else
                    {
                        if (File.Exists(exePath + "/config.json"))
                        {
                            customtext = File.ReadAllText(exePath + "/config.json");
                            dynamic _customtext = JsonConvert.DeserializeObject(customtext);
                            _overwritedetails = _customtext.Lobby.OverwriteDetails;
                            _overwritestate = _customtext.Lobby.OverwriteState;
                            _overwritelargeasset = _customtext.Lobby.OverwriteLargeAsset;
                            _overwritesmallasset = _customtext.Lobby.OverwriteSmallAsset;

                            _details = _customtext.Lobby.Details;
                            _state = _customtext.Lobby.State;
                            _largeasseturl = _customtext.Lobby.LargeAssetURL;
                            _largeassettext = _customtext.Lobby.LargeAssetText;
                            _smallasseturl = _customtext.Lobby.SmallAssetURL;
                            _smallassettext = _customtext.Lobby.SmallAssetText;

                            _newdetails = ReplaceTags.Lobby(_details);
                            _newstate = ReplaceTags.Lobby(_state);
                            _newlargeasseturl = ReplaceTags.Lobby(_largeasseturl);
                            _newlargeassettext = ReplaceTags.Lobby(_largeassettext);
                            _newsmallasseturl = ReplaceTags.Lobby(_smallasseturl);
                            _newsmallassettext = ReplaceTags.Lobby(_smallassettext);
                        }
                        else
                            ResetConfig();

                        if (_overwritelargeasset)
                            discord.UpdateLargeAsset(_newlargeasseturl, _newlargeassettext);
                        else
                        {
                            if (cb_ShowLevel.Checked)
                                discord.UpdateLargeAsset(_levelimage, $"Level {_level}");
                            else
                                discord.UpdateLargeAsset("logo", $"In Lobby");
                        }
                        if (_overwritesmallasset)
                            discord.UpdateSmallAsset(_newsmallasseturl, _newsmallassettext);
                        else
                            discord.UpdateSmallAsset("", "");
                        if (_overwritedetails)
                            discord.UpdateDetails(_newdetails);
                        else
                            discord.UpdateDetails($"{_nickname} » {_channel}");
                        if (_overwritestate)
                            discord.UpdateState(_newstate);
                        else
                            discord.UpdateState($"Level {_level}, XP: {_xp.ToString("#,##0")}/{_xprequired.ToString("#,##0")} ({_xppercentage}%)");
                        DiscordRPC.Button[] buttons = new DiscordRPC.Button[1];
                        buttons[0] = new DiscordRPC.Button { Label = "View Profile", Url = $"https://xero.gg/player/{_nickname}" };
                        discord.UpdateClearTime();
                        discord.UpdateButtons(buttons);
                    }
                }
            }
            catch
            {
                //Do nothing
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.accesskey = tb_accesskey.Text;
            Settings.Default.accesskeysecret = tb_accesskeysecret.Text;
            Settings.Default.windows = cb_StartWithWindows.Checked;
            Settings.Default.tray = cb_HideInTray.Checked;
            Settings.Default.showlevel = cb_ShowLevel.Checked;
            Settings.Default.Save();
        }

        private void cb_StartWithWindows_CheckedChanged(object sender, EventArgs e)
        {
            string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string shortcutPath = Path.Combine(startupFolder, "XeroPresence.lnk");

            if (cb_StartWithWindows.Checked)
            {
                // Create shortcut to the program's exe file in the startup folder
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = Application.ExecutablePath;
                shortcut.Save();
            }
            else
            {
                // Delete shortcut from the startup folder
                File.Delete(shortcutPath);
            }
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Visible = true;
                this.ShowInTaskbar = true;
                notifyIcon.Visible = false;
                cb_HideInTray.Checked = false;
            }
        }

        private void cb_HideInTray_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_HideInTray.Checked)
            {
                this.Visible = false;
                this.ShowInTaskbar = false;
                notifyIcon.Visible = true;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            tb_accesskey.Text = Settings.Default.accesskey;
            tb_accesskeysecret.Text = Settings.Default.accesskeysecret;
            cb_StartWithWindows.Checked = Settings.Default.windows;
            cb_HideInTray.Checked = Settings.Default.tray;
            cb_ShowLevel.Checked = Settings.Default.showlevel;
            HideInSystemTray();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (cb_StartWithWindows.Checked)
            {
                bt_login.Focus();
                bt_login.PerformClick();
            }
            //HideInSystemTray();
        }

        private void HideInSystemTray()
        {
            if (cb_StartWithWindows.Checked || cb_HideInTray.Checked)
            {
                this.Visible = false;
                this.ShowInTaskbar = false;
                notifyIcon.Visible = true;
            }
        }

        private void ResetConfig()
        {
            _overwritedetails = false;
            _overwritestate = false;
            _overwritelargeasset = false;
            _overwritesmallasset = false;

            _details = "";
            _state = "";
            _largeasseturl = "";
            _largeassettext = "";
            _smallasseturl = "";
            _smallassettext = "";

            _newdetails = "";
            _newstate = "";
            _newlargeasseturl = "";
            _newlargeassettext = "";
            _newsmallasseturl = "";
            _newsmallassettext = "";
        }

        private void LoadIniFile(string filePath)
        {
            iniData.Clear();
            string currentSection = "";

            try
            {
                foreach (string line in File.ReadLines(filePath, Encoding.UTF8))
                {
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        currentSection = line.Trim('[', ']');
                        iniData[currentSection] = new Dictionary<string, string>();
                    }
                    else if (!string.IsNullOrEmpty(currentSection))
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();
                            iniData[currentSection][key] = value;
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                // File was not found
            }

            foreach (var section in iniData.Keys)
            {
                cb_Profiles.Items.Add(section);
            }
        }

        private void bt_save_Click(object sender, EventArgs e)
        {
            Settings.Default.accesskey = tb_accesskey.Text;
            Settings.Default.accesskeysecret = tb_accesskeysecret.Text;
            Settings.Default.windows = cb_StartWithWindows.Checked;
            Settings.Default.tray = cb_HideInTray.Checked;
            Settings.Default.showlevel = cb_ShowLevel.Checked;
            Settings.Default.Save();
        }

        private void cb_Profiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedSection = cb_Profiles.SelectedItem.ToString();
            if (iniData.ContainsKey(selectedSection))
            {
                if (iniData[selectedSection].ContainsKey("AccessKeyID"))
                    tb_accesskey.Text = iniData[selectedSection]["AccessKeyID"];
                else
                    tb_accesskey.Text = "Key not found";

                if (iniData[selectedSection].ContainsKey("SecretAccessKey"))
                    tb_accesskeysecret.Text = iniData[selectedSection]["SecretAccessKey"];
                else
                    tb_accesskeysecret.Text = "Key not found";
                if (bt_login.Text == "Logged in")
                    MessageBox.Show("You seem to be already logged in to a different account.\nPlease restart the program to take affect.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}