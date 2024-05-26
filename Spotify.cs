using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using System.Reflection;
using System.Diagnostics;

namespace GenXdev.Helpers
{
    public static class Spotify
    {
        public static string VERSION = "0.5";
        public static UInt32 APPCOMMAND = 0x0319;
        public static IntPtr CMD_PLAYPAUSE = (IntPtr)917504;
        public static int CMD_MUTE = 524288;
        public static int CMD_VOLUMEDOWN = 589824;
        public static int CMD_VOLUMEUP = 655360;
        public static IntPtr CMD_STOP = (IntPtr)851968;
        public static int CMD_PREVIOUS = 786432;
        public static int CMD_NEXT = 720896;

        private static readonly object padlock = new object();
        private static readonly string clientId = "e7efbeae0f3f4409986218f5573ffd85";
        public static string RequestAuthenticationUri(int port)
        {
            var loginRequest = new LoginRequest(new Uri("http://localhost:" + port.ToString() + "/callback"), clientId, LoginRequest.ResponseType.Token)
            {
                Scope = new[] {
                    Scopes.UserReadPlaybackPosition,
                    Scopes.UserLibraryRead,
                    Scopes.UserLibraryModify,
                    Scopes.PlaylistModifyPrivate,
                    Scopes.PlaylistReadPrivate,
                    Scopes.UserFollowRead,
                    Scopes.PlaylistModifyPublic,
                    Scopes.AppRemoteControl,
                    Scopes.UserReadCurrentlyPlaying,
                    Scopes.UserModifyPlaybackState,
                    Scopes.UserReadPlaybackState,
                    Scopes.PlaylistReadCollaborative,
                    Scopes.UserFollowModify
                }
            };

            return loginRequest.ToUri().ToString();
        }

        public static string RequestAuthenticationTokenUsingOAuth(int port)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var allResourceNames = assembly.GetManifestResourceNames();
            foreach (var resourceName in allResourceNames)
            {
                Console.WriteLine(resourceName);
            }

            EmbedIOAuthServer server = null;
            string token = null;
            async Task OnAuthorizationCodeReceived(object sender, AuthorizationCodeResponse response)
            {
                await Task.Delay(1);
                lock (padlock)
                {
                    token = response.Code;
                }
            }
            try
            {
                server = new EmbedIOAuthServer(
                    new Uri("http://localhost:" + port + "/callback"),
                    port,
                    Assembly.GetExecutingAssembly(),
                    "GenXdev.Helpers.custom_site"
                );

                server.Start().Wait();
                server.AuthorizationCodeReceived += OnAuthorizationCodeReceived;
                System.Console.TreatControlCAsInput = true;

                while (true)
                {
                    lock (padlock)
                    {
                        if (token != null)
                        {
                            return token;
                        }
                    }

                    System.Threading.Thread.Sleep(100);
                }
            }
            finally
            {
                if (server != null)
                {
                    server.AuthorizationCodeReceived -= OnAuthorizationCodeReceived;
                    server.Stop().Wait();
                    server = null;
                }
            }
        }

        public static void TransferPlayback(string apiKey, string deviceId)
        {
            var deviceIds = new List<string>();
            deviceIds.Add(deviceId);
            var client = new SpotifyClient(apiKey);
            client.Player.TransferPlayback(new PlayerTransferPlaybackRequest(deviceIds)).Wait();
        }

        public static void Stop(string apiKey)
        {
            var client = new SpotifyClient(apiKey);
            client.Player.SeekTo(new PlayerSeekToRequest(0) { DeviceId = GetActiveDeviceId(client, apiKey) }).Wait();
            client.Player.PausePlayback(new PlayerPausePlaybackRequest() { DeviceId = GetActiveDeviceId(client, apiKey) }).Wait();
        }

        public static void Start(string apiKey)
        {
            var client = new SpotifyClient(apiKey);
            client.Player.ResumePlayback(new PlayerResumePlaybackRequest() { DeviceId = GetActiveDeviceId(client, apiKey) }).Wait();
        }

        public static void Pause(string apiKey)
        {
            var client = new SpotifyClient(apiKey);
            client.Player.PausePlayback(new PlayerPausePlaybackRequest() { DeviceId = GetActiveDeviceId(client, apiKey) }).Wait();
        }

        public static void Previous(string apiKey)
        {
            var client = new SpotifyClient(apiKey);
            client.Player.SkipPrevious(new PlayerSkipPreviousRequest() { DeviceId = GetActiveDeviceId(client, apiKey) }).Wait();
        }

        public static void Next(string apiKey)
        {
            var client = new SpotifyClient(apiKey);
            client.Player.SkipNext(new PlayerSkipNextRequest() { DeviceId = GetActiveDeviceId(client, apiKey) }).Wait();
        }
        public static void RepeatSong(string apiKey)
        {
            var client = new SpotifyClient(apiKey);
            client.Player.SetRepeat(new PlayerSetRepeatRequest(PlayerSetRepeatRequest.State.Track) { DeviceId = GetActiveDeviceId(client, apiKey) }).Wait();
        }
        public static void RepeatContext(string apiKey)
        {
            var client = new SpotifyClient(apiKey);
            client.Player.SetRepeat(new PlayerSetRepeatRequest(PlayerSetRepeatRequest.State.Context) { DeviceId = GetActiveDeviceId(client, apiKey) }).Wait();
        }
        public static void RepeatOff(string apiKey)
        {
            var client = new SpotifyClient(apiKey);
            client.Player.SetRepeat(new PlayerSetRepeatRequest(PlayerSetRepeatRequest.State.Off) { DeviceId = GetActiveDeviceId(client, apiKey) }).Wait();
        }
        public static void ShuffleOn(string apiKey)
        {
            var client = new SpotifyClient(apiKey);
            client.Player.SetShuffle(new PlayerShuffleRequest(true) { DeviceId = GetActiveDeviceId(client, apiKey) }).Wait();
        }
        public static void ShuffleOff(string apiKey)
        {
            var client = new SpotifyClient(apiKey);
            client.Player.SetShuffle(new PlayerShuffleRequest(false) { DeviceId = GetActiveDeviceId(client, apiKey) }).Wait();
        }

        public static List<Device> GetDevices(string apiKey)
        {
            var client = new SpotifyClient(apiKey);
            GetActiveDeviceId(client, apiKey);
            return client.Player.GetAvailableDevices().Result.Devices;
        }

        static string GetActiveDeviceId(SpotifyClient client, string apiKey)
        {
            var devices = client.Player.GetAvailableDevices().Result.Devices;
            
            foreach (var device in devices)
            {
                if (device.IsActive) return device.Id;
            }

            foreach (var device in devices)
            {
                return device.Id;
            }

            var processes = Process.GetProcessesByName("Spotify");

            if (processes.Length == 0)
            {
                var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "Spotify\\Spotify.exe");

                if (File.Exists(path))
                {
                    Process.Start(path);
                    System.Threading.Thread.Sleep(2000);
                    processes = Process.GetProcessesByName("Spotify");
                }
            }

            foreach (var process in processes)
            {
                if (process.MainWindowHandle == IntPtr.Zero) continue;

                var windows = GenXdev.Helpers.WindowObj.GetMainWindow(process);

                foreach (var w in windows)
                {
                    w.SendMessage(APPCOMMAND, IntPtr.Zero, CMD_PLAYPAUSE);
                    System.Threading.Thread.Sleep(500);
                    w.SendMessage(APPCOMMAND, IntPtr.Zero, CMD_STOP);
                }
            }

            devices = client.Player.GetAvailableDevices().Result.Devices;

            foreach (var device in devices)
            {
                if (device.IsActive) return device.Id;
            }

            foreach (var device in devices)
            {
                return device.Id;
            }

            return null;
        }

        public static List<Device> SetActiveDevice(string apiKey, string deviceId)
        {
            var client = new SpotifyClient(apiKey);
            client.Player.TransferPlayback(new PlayerTransferPlaybackRequest(new string[1] { deviceId })).Wait(); 
            return client.Player.GetAvailableDevices().Result.Devices;
        }

        public static SearchResponse Search(string apiKey, string Query, int SearchType = (int)SearchRequest.Types.Track)
        {
            var client = new SpotifyClient(apiKey);
            SearchResponse foundTracks = client.Search.Item(new SearchRequest((SearchRequest.Types)SearchType, Query)).Result;
            return foundTracks;
        }


        public static object SearchAndPlay(string apiKey, string Query, int SearchType = (int)SearchRequest.Types.Track)
        {
            var client = new SpotifyClient(apiKey);
            SearchResponse foundTracks = client.Search.Item(new SearchRequest((SearchRequest.Types)SearchType, Query)).Result;
            var Uris = new List<string>();

            if (((((SearchType & ((int)SearchRequest.Types.Artist)) > 0) || (SearchType & ((int)SearchRequest.Types.All)) > 0)) &&
              foundTracks.Artists != null && foundTracks.Artists.Items != null && foundTracks.Artists.Items.Count > 0)
            {
                var albums = client.Artists.GetAlbums(foundTracks.Artists.Items[0].Uri).Result.Items;

                var uris = (from q in albums select q.Uri.ToString()).ToArray<string>();

                client.Player.ResumePlayback(new PlayerResumePlaybackRequest()
                {
                    Uris = uris,
                    DeviceId = GetActiveDeviceId(client, apiKey)
                }).Wait();

                return new
                {
                    Artist = foundTracks.Artists.Items[0],
                    Albums = albums
                };
            }

            if (((((SearchType & ((int)SearchRequest.Types.Track)) > 0) || (SearchType & ((int)SearchRequest.Types.All)) > 0)) &&
                foundTracks.Tracks != null && foundTracks.Tracks.Items != null && foundTracks.Tracks.Items.Count > 0)
            {
                var item = foundTracks.Tracks.Items[0];

                client.Player.ResumePlayback(new PlayerResumePlaybackRequest()
                {
                    Uris = new string[1] { item.Uri },
                    DeviceId = GetActiveDeviceId(client, apiKey)
                }).Wait();

                return new
                {
                    Artist = item.Artists[0].Name,
                    Album = item.Album.Name,
                    Track = item.Name
                };
            }

            if (((((SearchType & ((int)SearchRequest.Types.Album)) > 0) || (SearchType & ((int)SearchRequest.Types.All)) > 0)) &&
                foundTracks.Albums != null && foundTracks.Albums.Items != null && foundTracks.Albums.Items.Count > 0)
            {
                var item = foundTracks.Albums.Items[0];

                client.Player.ResumePlayback(new PlayerResumePlaybackRequest()
                {
                    ContextUri = item.Uri,
                    DeviceId = GetActiveDeviceId(client, apiKey)
                }).Wait();

                return new
                {
                    Artist = item.Artists[0].Name,
                    Album = item.Name
                };
            }

            if (((((SearchType & ((int)SearchRequest.Types.Episode)) > 0) || (SearchType & ((int)SearchRequest.Types.All)) > 0)) &&
                foundTracks.Episodes != null && foundTracks.Episodes.Items != null && foundTracks.Episodes.Items.Count > 0)
            {
                var item = foundTracks.Episodes.Items[0];

                client.Player.ResumePlayback(new PlayerResumePlaybackRequest()
                {
                    ContextUri = item.Uri,
                    DeviceId = GetActiveDeviceId(client, apiKey)
                }).Wait();

                return new
                {
                    Episode = item.Name,
                    Description = item.Description,
                };
            }

            if (((((SearchType & ((int)SearchRequest.Types.Show)) > 0) || (SearchType & ((int)SearchRequest.Types.All)) > 0)) &&
                foundTracks.Shows != null && foundTracks.Shows.Items != null && foundTracks.Shows.Items.Count > 0)
            {
                var item = foundTracks.Shows.Items[0];

                client.Player.ResumePlayback(new PlayerResumePlaybackRequest()
                {
                    ContextUri = item.Uri,
                    DeviceId = GetActiveDeviceId(client, apiKey)
                }).Wait();

                return new
                {
                    Publisher = item.Publisher,
                    Show = item.Name
                };
            }

            if (((((SearchType & ((int)SearchRequest.Types.Playlist)) > 0) || (SearchType & ((int)SearchRequest.Types.All)) > 0)) &&
                foundTracks.Playlists != null && foundTracks.Playlists.Items != null && foundTracks.Playlists.Items.Count > 0)
            {
                var item = foundTracks.Playlists.Items[0];

                client.Player.ResumePlayback(new PlayerResumePlaybackRequest()
                {
                    ContextUri = item.Uri,
                    DeviceId = GetActiveDeviceId(client, apiKey)
                }).Wait();

                return new
                {
                    Artist = item.Owner.DisplayName,
                    Playlist = item.Name
                };
            }

            return null;
        }

        public static List<FullPlaylist> GetUserPlaylists(string apiKey, string[] filters)
        {
            var client = new SpotifyClient(apiKey);
            var playlists = new List<FullPlaylist>();

            Paging<FullPlaylist> results = client.Playlists.CurrentUsers().Result;

            if (results.Items == null)
            {
                return playlists;
            }

            while (results.Items.Count > 0)
            {
                foreach (var item in results.Items)
                {
                    var found = true;

                    if (filters != null)
                    {
                        found = false;

                        foreach (var filter in filters)
                        {
                            var re = new System.Text.RegularExpressions.Regex(System.Text.RegularExpressions.Regex.Escape(filter).Replace("\\*", ".*"), System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                            if (re.Match(item.Id).Success || re.Match(item.Uri).Success || re.Match(item.Name).Success)
                            {
                                found = true;
                                break;
                            }
                        }
                    }

                    if (!found) continue;

                    playlists.Add(item);

                    var firstTrackItemRef = item.Tracks;
                    var trackItemRef = item.Tracks;

                    if (trackItemRef.Items == null)
                    {
                        trackItemRef.Items = new List<PlaylistTrack<IPlayableItem>>();
                    }

                    trackItemRef = client.Playlists.GetItems(item.Id, new PlaylistGetItemsRequest(PlaylistGetItemsRequest.AdditionalTypes.All)).Result;

                    if (trackItemRef.Items == null)
                    {
                        continue;
                    }

                    while (trackItemRef.Items.Count > 0)
                    {
                        if (trackItemRef != firstTrackItemRef)
                        {
                            foreach (var track in trackItemRef.Items)
                            {
                                firstTrackItemRef.Items.Add(track);
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(trackItemRef.Next))
                        {
                            trackItemRef = client.Playlists.GetItems(item.Id, new PlaylistGetItemsRequest(PlaylistGetItemsRequest.AdditionalTypes.All)
                            {
                                Offset = trackItemRef.Offset + trackItemRef.Items.Count
                            }).Result;

                            if (trackItemRef.Items == null) break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(results.Next))
                {
                    results = client.Playlists.CurrentUsers(new PlaylistCurrentUsersRequest() { Offset = results.Offset + results.Items.Count }).Result;
                }
                else
                {
                    break;
                }
            }

            return playlists;
        }

        public static void AddToPlaylist(string apiKey, string playlistId, string[] uris)
        {
            var client = new SpotifyClient(apiKey);
            var tracks = new List<string>(uris);

            while (tracks.Count > 0)
            {
                List<string> currentBatch = tracks.GetRange(0, Math.Min(50, tracks.Count));
                tracks.RemoveRange(0, currentBatch.Count);

                client.Playlists.AddItems(playlistId, new PlaylistAddItemsRequest(currentBatch)).Wait();
            }
        }
        public static List<SavedTrack> GetLibraryTracks(string apiKey)
        {
            var client = new SpotifyClient(apiKey);
            var tracks = new List<SavedTrack>();

            Paging<SavedTrack> results = client.Library.GetTracks().Result;
            while (results.Items.Count > 0)
            {
                foreach (var item in results.Items)
                {
                    tracks.Add(item);
                }

                if (!string.IsNullOrWhiteSpace(results.Next))
                {
                    results = client.Library.GetTracks(new LibraryTracksRequest() { Offset = results.Offset + results.Items.Count }).Result;
                }
                else
                {
                    break;
                }
            }

            return tracks;
        }

        public static FullPlaylist NewPlaylist(string apiKey, string name, bool publicPlaylist = false, bool collabrative = false, string description = "")
        {
            var client = new SpotifyClient(apiKey);
            var currentUser = client.UserProfile.Current().Result;

            return client.Playlists.Create(currentUser.Id, new PlaylistCreateRequest(name)
            {
                Collaborative = collabrative,
                Description = string.IsNullOrWhiteSpace(description) ? null : description,
                Public = publicPlaylist
            }).Result;
        }

        public static void ChangePlaylistDetails(string playlistId, string apiKey, string name = "", bool? publicPlaylist = null, bool? collabrative = null, string description = "")
        {
            var client = new SpotifyClient(apiKey);

            client.Playlists.ChangeDetails(playlistId,
                new PlaylistChangeDetailsRequest()
                {
                    Collaborative = collabrative,
                    Description = string.IsNullOrWhiteSpace(description) ? null : description,
                    Public = publicPlaylist,
                    Name = string.IsNullOrWhiteSpace(name) ? null : name,
                }
            );
        }

        public static void RemoveFromPlaylist(string apiKey, string playlistId, string[] uris)
        {
            var client = new SpotifyClient(apiKey);
            var tracks = (from q in uris select new PlaylistRemoveItemsRequest.Item() { Uri = q }).ToList<PlaylistRemoveItemsRequest.Item>();

            while (tracks.Count > 0)
            {
                List<PlaylistRemoveItemsRequest.Item> currentBatch = tracks.GetRange(0, Math.Min(50, tracks.Count));
                tracks.RemoveRange(0, currentBatch.Count);

                client.Playlists.RemoveItems(playlistId, new PlaylistRemoveItemsRequest() { Tracks = currentBatch }).Wait();
            }
        }

        public static void RemoveFromLiked(string apiKey, string[] trackIds)
        {
            var client = new SpotifyClient(apiKey);
            var tracks = new List<string>(trackIds);

            while (tracks.Count > 0)
            {
                List<string> currentBatch = tracks.GetRange(0, Math.Min(50, tracks.Count));
                tracks.RemoveRange(0, currentBatch.Count);

                client.Library.RemoveTracks(new LibraryRemoveTracksRequest(currentBatch)).Wait();
            }
        }

        public static void AddToLiked(string apiKey, string[] trackIds)
        {
            var client = new SpotifyClient(apiKey);
            var tracks = new List<string>(trackIds);

            while (tracks.Count > 0)
            {
                List<string> currentBatch = tracks.GetRange(0, Math.Min(50, tracks.Count));
                tracks.RemoveRange(0, currentBatch.Count);

                client.Library.SaveTracks(new LibrarySaveTracksRequest(currentBatch)).Wait();
            }
        }

        public static CurrentlyPlaying GetCurrentlyPlaying(string apiKey)
        {
            var client = new SpotifyClient(apiKey);

            return client.Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest(PlayerCurrentlyPlayingRequest.AdditionalTypes.All)).Result;
        }
        public static List<TrackAudioFeatures> GetSeveralAudioFeatures(string apiKey, string[] trackIds)
        {
            var client = new SpotifyClient(apiKey);

            return client.Tracks.GetSeveralAudioFeatures(new TracksAudioFeaturesRequest(trackIds)).Result.AudioFeatures;
        }
        public static TrackAudioFeatures GetSeveralAudioFeatures(string apiKey, string trackId)
        {
            var client = new SpotifyClient(apiKey);

            return client.Tracks.GetAudioFeatures(trackId).Result;
        }
        public static FullTrack GetTrackById(string apiKey, string trackId)
        {
            var client = new SpotifyClient(apiKey);

            return client.Tracks.Get(trackId).Result;
        }

        public static void ReorderPlaylist(string apiKey, string playlistId, int RangeStart, int InsertBefore, int? RangeLength)
        {
            var client = new SpotifyClient(apiKey);

            client.Playlists.ReorderItems(playlistId, new PlaylistReorderItemsRequest(RangeStart, InsertBefore)
            {
                RangeLength = RangeLength
            });
        }

        public static List<PlaylistTrack<IPlayableItem>> GetPlaylistTracks(string apiKey, string playlistId)
        {
            var client = new SpotifyClient(apiKey);
            var tracks = new List<PlaylistTrack<IPlayableItem>>();

            Paging<PlaylistTrack<IPlayableItem>> results = client.Playlists.GetItems(playlistId).Result;
            while (results.Items.Count > 0)
            {
                foreach (var item in results.Items)
                {
                    tracks.Add(item);
                }

                if (!string.IsNullOrWhiteSpace(results.Next))
                {
                    results = client.Playlists.GetItems(playlistId, new PlaylistGetItemsRequest() { Offset = results.Offset + results.Items.Count }).Result;
                }
                else
                {
                    break;
                }
            }

            return tracks;
        }

        public static object SearchAndAdd(string apiKey, string Query, int SearchType = (int)SearchRequest.Types.Track)
        {
            var client = new SpotifyClient(apiKey);
            SearchResponse foundTracks = client.Search.Item(new SearchRequest((SearchRequest.Types)SearchType, Query)).Result;
            var Uris = new List<string>();

            if (((((SearchType & ((int)SearchRequest.Types.Track)) > 0) || (SearchType & ((int)SearchRequest.Types.All)) > 0)) &&
                foundTracks.Tracks != null && foundTracks.Tracks.Items != null && foundTracks.Tracks.Items.Count > 0)
            {
                var item = foundTracks.Tracks.Items[0];

                client.Player.AddToQueue(new PlayerAddToQueueRequest(item.Uri) { DeviceId = GetActiveDeviceId(client, apiKey) }).Wait();

                return new
                {
                    Artist = item.Artists[0].Name,
                    Album = item.Album.Name,
                    Track = item.Name
                };
            }

            if (((((SearchType & ((int)SearchRequest.Types.Album)) > 0) || (SearchType & ((int)SearchRequest.Types.All)) > 0)) &&
                foundTracks.Albums != null && foundTracks.Albums.Items != null && foundTracks.Albums.Items.Count > 0)
            {
                var item = foundTracks.Albums.Items[0];

                client.Player.AddToQueue(new PlayerAddToQueueRequest(item.Uri) { DeviceId = GetActiveDeviceId(client, apiKey) }).Wait();

                return new
                {
                    Artist = item.Artists[0].Name,
                    Album = item.Name
                };
            }

            if (((((SearchType & ((int)SearchRequest.Types.Episode)) > 0) || (SearchType & ((int)SearchRequest.Types.All)) > 0)) &&
                foundTracks.Episodes != null && foundTracks.Episodes.Items != null && foundTracks.Episodes.Items.Count > 0)
            {
                var item = foundTracks.Episodes.Items[0];

                client.Player.AddToQueue(new PlayerAddToQueueRequest(item.Uri) { DeviceId = GetActiveDeviceId(client, apiKey) }).Wait();

                return new
                {
                    Episode = item.Name,
                    Description = item.Description,
                };
            }

            if (((((SearchType & ((int)SearchRequest.Types.Show)) > 0) || (SearchType & ((int)SearchRequest.Types.All)) > 0)) &&
                foundTracks.Shows != null && foundTracks.Shows.Items != null && foundTracks.Shows.Items.Count > 0)
            {
                var item = foundTracks.Shows.Items[0];

                client.Player.AddToQueue(new PlayerAddToQueueRequest(item.Uri) { DeviceId = GetActiveDeviceId(client, apiKey) }).Wait();

                return new
                {
                    Publisher = item.Publisher,
                    Show = item.Name
                };
            }

            if (((((SearchType & ((int)SearchRequest.Types.Playlist)) > 0) || (SearchType & ((int)SearchRequest.Types.All)) > 0)) &&
                foundTracks.Playlists != null && foundTracks.Playlists.Items != null && foundTracks.Playlists.Items.Count > 0)
            {
                var item = foundTracks.Playlists.Items[0];

                client.Player.AddToQueue(new PlayerAddToQueueRequest(item.Uri) { DeviceId = GetActiveDeviceId(client, apiKey) }).Wait();

                return new
                {
                    Artist = item.Owner.DisplayName,
                    Playlist = item.Name
                };
            }

            return null;
        }

        /*
         

# include <windows.h>
# include <iostream>

#define VERSION "0.5"

#define APPCOMMAND 0x0319

#define CMD_PLAYPAUSE 917504
#define CMD_MUTE 524288
#define CMD_VOLUMEDOWN 589824
#define CMD_VOLUMEUP 655360
#define CMD_STOP 851968
#define CMD_PREVIOUS 786432
#define CMD_NEXT 720896

        int main(int argc, char** argv)
        {
            if (argc > 1)
            {
                HWND window_handle = FindWindow("SpotifyMainWindow", NULL);

                if (window_handle == NULL)
                {
                    std::cout << "ERROR" << std::endl;
                    std::cout << "Can not find spotify, is it running?" << std::endl;
                    return 1;
                }

                char buffer[512] = "";
                std::string artistName = "";
                std::string songName = "";

                if (GetWindowText(window_handle, buffer, sizeof(buffer)) > 0)
                {
                    std::string title = buffer;

                    std::string::size_type pos1 = title.find('-');
                    std::string::size_type pos2 = title.find(static_cast<char>(-106));

                    if (pos1 != std::string::npos && pos2 != std::string::npos)
			{
                        pos1 += 2;
                        artistName = title.substr(pos1, pos2 - pos1 - 1);
                        songName = title.substr(pos2 + 2);
                    }
                }

                std::string command = argv[1];

                if (command == "playpause")
                {
                    SendMessage(window_handle, APPCOMMAND, 0, CMD_PLAYPAUSE);
                    std::cout << "OK" << std::endl;
                    return 0;
                }
                else if (command == "next")
                {
                    SendMessage(window_handle, APPCOMMAND, 0, CMD_NEXT);
                    std::cout << "OK" << std::endl;
                    return 0;
                }
                else if (command == "prev")
                {
                    SendMessage(window_handle, APPCOMMAND, 0, CMD_PREVIOUS);
                    std::cout << "OK" << std::endl;
                    return 0;
                }
                else if (command == "stop")
                {
                    SendMessage(window_handle, APPCOMMAND, 0, CMD_STOP);
                    std::cout << "OK" << std::endl;
                    return 0;
                }
                else if (command == "mute")
                {
                    SendMessage(window_handle, APPCOMMAND, 0, CMD_MUTE);
                    std::cout << "OK" << std::endl;
                    return 0;
                }
                else if (command == "volup")
                {
                    SendMessage(window_handle, APPCOMMAND, 0, CMD_VOLUMEUP);
                    std::cout << "OK" << std::endl;
                    return 0;
                }
                else if (command == "voldown")
                {
                    SendMessage(window_handle, APPCOMMAND, 0, CMD_VOLUMEDOWN);
                    std::cout << "OK" << std::endl;
                    return 0;
                }
                else if (command == "status")
                {
                    if (artistName == "" && songName == "")
                    {
                        std::cout << "WARN" << std::endl;
                        std::cout << "Playback paused" << std::endl;
                    }
                    else
                    {
                        std::cout << "OK" << std::endl;
                        std::cout << artistName << std::endl;
                        std::cout << songName << std::endl;
                    }

                    return 0;
                }
            }

            std::cout << "spotify_cmd version " << VERSION << ", copyright by Mattias Runge 2009" << std::endl;
            std::cout << "Usage:" << " ./spotify_cmd [playpause|prev|next|stop|mute|volup|voldown|status]" << std::endl;
            return 0;
        }
*/
    }
}
