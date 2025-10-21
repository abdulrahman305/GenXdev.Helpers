// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : Spotify.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.304.2025
// ################################################################################
// Copyright (c)  René Vaessen / GenXdev
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ################################################################################



using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using System.Diagnostics;
using System.Reflection;

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
            TextWriter originalOut = Console.Out; // Save the original Console.Out
            TextWriter originalError = Console.Error; // Save the original Console.Error

            try
            {
                Console.SetOut(TextWriter.Null); // Suppress output to stdout
                Console.SetError(TextWriter.Null); // Suppress output to stderr

                var assembly = Assembly.GetExecutingAssembly();
                var allResourceNames = assembly.GetManifestResourceNames();
                foreach (var resourceName in allResourceNames)
                {
                    // Suppressed output
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

                    // Schedule a task to restore the console streams after a short delay
                    Task.Run(async () =>
                    {
                        await Task.Delay(100); // Adjust delay as needed
                        Console.SetOut(originalOut); // Restore the original Console.Out
                        Console.SetError(originalError); // Restore the original Console.Error
                    });
                }
            }
            finally
            {
                // Ensure the console streams are restored in case of an exception
                Task.Run(async () =>
                {
                    await Task.Delay(100); // Adjust delay as needed
                    Console.SetOut(originalOut); // Restore the original Console.Out
                    Console.SetError(originalError); // Restore the original Console.Error
                });
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

        public static void ClearQueue(string apiKey)
        {
            var client = new SpotifyClient(apiKey);
            // The Spotify Web API does not offer a direct way to clear the playback queue.
            // This workaround re-starts playback with only the currently playing track,
            // effectively discarding any queued items.
            try
            {
                var currentlyPlaying = client.Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest(PlayerCurrentlyPlayingRequest.AdditionalTypes.All)).Result;
                if (currentlyPlaying?.Item != null)
                {
                    client.Player.ResumePlayback(new PlayerResumePlaybackRequest()
                    {
                        Uris = new string[0],
                        DeviceId = GetActiveDeviceId(client, apiKey)
                    }).Wait();
                }
            }
            catch // (Exception ex)
            {
            }
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

        public static SearchResponse Search(string apiKey, string Query, SpotifyAPI.Web.SearchRequest.Types SearchType = SearchRequest.Types.Track)
        {
            var client = new SpotifyClient(apiKey);
            SearchResponse foundTracks = client.Search.Item(new SearchRequest(SearchType, Query)).Result;
            return foundTracks;
        }


        public static object SearchAndPlay(string apiKey, string Query, SpotifyAPI.Web.SearchRequest.Types SearchType = SearchRequest.Types.Track)
        {
            var client = new SpotifyClient(apiKey);
            SearchResponse foundTracks = client.Search.Item(new SearchRequest(SearchType, Query)).Result;
            var Uris = new List<string>();

            try
            {
                if ((SearchType & SearchRequest.Types.Artist) > 0 &&
                    foundTracks.Artists != null && foundTracks.Artists.Items != null && foundTracks.Artists.Items.Count > 0)
                {
                    var artist = foundTracks.Artists.Items[0];
                    var albums = client.Artists.GetAlbums(artist.Id).Result.Items;

                    var uris = new List<string>();
                    foreach (var album in albums)
                    {
                        var albumTracks = client.Albums.GetTracks(album.Id).Result.Items;
                        uris.AddRange(albumTracks.Select(track => track.Uri));
                    }

                    // Log URIs for debugging
                    client.Player.ResumePlayback(new PlayerResumePlaybackRequest()
                    {
                        Uris = uris.ToArray(),
                        DeviceId = GetActiveDeviceId(client, apiKey)
                    }).Wait();

                    return new
                    {
                        Artist = artist,
                        Albums = albums
                    };
                }

                if ((SearchType & SearchRequest.Types.Track) > 0 &&
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

                if ((SearchType & SearchRequest.Types.Album) > 0 &&
                    foundTracks.Albums != null && foundTracks.Albums.Items != null && foundTracks.Albums.Items.Count > 0)
                {
                    var item = foundTracks.Albums.Items[0];
                    var albumTracks = client.Albums.GetTracks(item.Id).Result.Items;
                    var uris = albumTracks.Select(track => track.Uri).ToArray();

                    client.Player.ResumePlayback(new PlayerResumePlaybackRequest()
                    {
                        Uris = uris,
                        DeviceId = GetActiveDeviceId(client, apiKey)
                    }).Wait();

                    return new
                    {
                        Artist = item.Artists[0].Name,
                        Album = item.Name
                    };
                }

                if ((SearchType & SearchRequest.Types.Episode) > 0 &&
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

                if ((SearchType & SearchRequest.Types.Show) > 0 &&
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

                if ((SearchType & SearchRequest.Types.Playlist) > 0 &&
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
            }
            catch
            {
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
                List<string> currentBatch = tracks.GetRange(0, System.Math.Min(50, tracks.Count));
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
                List<PlaylistRemoveItemsRequest.Item> currentBatch = tracks.GetRange(0, System.Math.Min(50, tracks.Count));
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
                List<string> currentBatch = tracks.GetRange(0, System.Math.Min(50, tracks.Count));
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

        public static object SearchAndAdd(string apiKey, string Query, SpotifyAPI.Web.SearchRequest.Types SearchType = SearchRequest.Types.Track)
        {
            var client = new SpotifyClient(apiKey);
            SearchResponse results = client.Search.Item(new SearchRequest(SearchType, Query)).Result;
            var Uris = new List<string>();

            if ((SearchType & (SearchRequest.Types.Track)) > 0 && results.Tracks?.Items?.Count > 0)
            {
                var item = results.Tracks.Items[0];
                client.Player.AddToQueue(new PlayerAddToQueueRequest(item.Uri) { DeviceId = GetActiveDeviceId(client, apiKey) }).Wait();
                return new
                {
                    Artist = item.Artists[0].Name,
                    Album = item.Album.Name,
                    Track = item.Name
                };
            }

            if ((SearchType & (SearchRequest.Types.Album)) > 0 && results.Albums?.Items?.Count > 0)
            {
                var item = results.Albums.Items[0];
                client.Player.AddToQueue(new PlayerAddToQueueRequest(item.Uri) { DeviceId = GetActiveDeviceId(client, apiKey) }).Wait();
                return new
                {
                    Artist = item.Artists[0].Name,
                    Album = item.Name
                };
            }

            if ((SearchType & (SearchRequest.Types.Episode)) > 0 && results.Episodes?.Items?.Count > 0)
            {
                var item = results.Episodes.Items[0];
                client.Player.AddToQueue(new PlayerAddToQueueRequest(item.Uri) { DeviceId = GetActiveDeviceId(client, apiKey) }).Wait();
                return new
                {
                    Episode = item.Name,
                    Description = item.Description,
                };
            }

            if ((SearchType & (SearchRequest.Types.Show)) > 0 && results.Shows?.Items?.Count > 0)
            {
                var item = results.Shows.Items[0];
                client.Player.AddToQueue(new PlayerAddToQueueRequest(item.Uri) { DeviceId = GetActiveDeviceId(client, apiKey) }).Wait();
                return new
                {
                    Publisher = item.Publisher,
                    Show = item.Name
                };
            }

            if ((SearchType & (SearchRequest.Types.Playlist)) > 0 && results.Playlists?.Items?.Count > 0)
            {
                var item = results.Playlists.Items[0];
                client.Player.AddToQueue(new PlayerAddToQueueRequest(item.Uri) { DeviceId = GetActiveDeviceId(client, apiKey) }).Wait();
                return new
                {
                    Artist = item.Owner.DisplayName,
                    Playlist = item.Name
                };
            }

            return null;
        }
    }
}