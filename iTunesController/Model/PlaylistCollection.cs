using System.Collections.Generic;
using iTunesLib;

namespace Schober.Felix.ITunes.Controller.Model
{
    
    public class PlaylistCollection
    {
        public string Name { get; }

        private readonly IITPlaylist _playlist;
        public Playlist Playlist { get; private set; }
        private PlaylistCollection _parent;

        public List<PlaylistCollection> PlaylistNodes { get; }
        public int Nodes => PlaylistNodes.Count;

        public PlaylistCollection()
        {
            PlaylistNodes = new List<PlaylistCollection>();
            Name = "root";
            _playlist = null;
            Playlist = null;
            _parent = null;
        }
        private PlaylistCollection(IITPlaylist playlist, PlaylistCollection parent)
        {
            PlaylistNodes = new List<PlaylistCollection>();
            _playlist = playlist;
            Playlist = new Playlist(_playlist);
            Name = Playlist.Name;
            _parent = parent;
        }

        public void InsertNode(IITPlaylist playlist)
        {
            var parentNode = SearchForParentNode(playlist);
            var node = new PlaylistCollection(playlist, parentNode);

            // if we could not find a parent this is the root element.
            if (parentNode == null)
            {
                parentNode = this;
            }
            // add the node to the parent node
            node._parent = parentNode;
            parentNode.PlaylistNodes.Add(node);
        }

        private PlaylistCollection SearchForParentNode(IITPlaylist playlist)
        {
            // This won't work for non user playlists
            if (!(playlist is IITUserPlaylist userPlaylist)) return null;

            return SearchForParentNode(userPlaylist);
        }

        private PlaylistCollection SearchForParentNode(IITUserPlaylist playlist)
        {
            // if the playlist doesn't have a parent return null to signal that.
            if (playlist.get_Parent() == null) return null;

            // if the current object (this) is the parent node -> return
            if (Equals(playlist.get_Parent(), this))
            {
                return this;
            }

            // perform the search one level deeper
            foreach (var playlistCollectionNode in PlaylistNodes)
            {
                var parentNode = playlistCollectionNode.SearchForParentNode(playlist);

                // did we find the parent node?
                if (parentNode != null)
                {
                    return parentNode;
                }
            }
            // we've searched all child nodes and could still not find the parent node.
            // if the current node doesn't have a parent anymore, we've reached the end and won't continue our search.
            //if (_parent == null) return null;
            //return _parent.SearchForParentNode(playlist);
            return null;
        }

        private static bool Equals(IITUserPlaylist x, PlaylistCollection y)
        {
            if (x == null && y == null || x == null && y.Name == "root") return true;
            if (x == null) return false;
            if (y == null) return false;

            return x.Name.Equals(y.Name) && Equals(x.get_Parent(), y._parent);

        }

        public override string ToString()
        {
            return Name + "(" + _parent + ")";
        }
    }
}
