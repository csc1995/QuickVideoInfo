﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using YTII.ModelFactory.Models;
using Android.Util;

namespace YTII.Droid.App.Caches
{
    public class VideoModelCache<T> : IVideoModelCache<T>
        where T : IVideoModel
    {
        static private Dictionary<string, T> _list = new Dictionary<string, T>();

        static private Queue<string> _idOrderQueue = new Queue<string>(20);

        private const int MaxItems = 20;

        internal int ItemCount { get => _list.Count; }

        private static int _cacheHits = 0;
        internal int CacheHits { get => _cacheHits; private set => _cacheHits = value; }

        private static int _cacheMisses = 0;
        internal int CacheMisses { get => _cacheMisses; private set => _cacheMisses = value; }


        public void Add(T item)
        {
            if (item == null || string.IsNullOrEmpty(item.VideoId))
            {
                Log.Info($"YTII.{nameof(YouTubeModelCache)}.{nameof(Add)}", $"Cannot Add Null Item/VideoId");
                return;
            }

            if (_list.Count >= MaxItems)
                _list.Remove(_idOrderQueue.Dequeue());

            if (!_idOrderQueue.Contains(item.VideoId))
                _idOrderQueue.Enqueue(item.VideoId);

            if (!_list.ContainsKey(item.VideoId))
                _list.Add(item.VideoId, item);

            Log.Info($"YTII.{nameof(YouTubeModelCache)}.{nameof(Add)}", $"Cache Item Added");
        }

        public bool IsCached(string videoId)
        {
            var isCached = _list.ContainsKey(videoId);

            if (isCached)
                CacheHits++;
            else
                CacheMisses++;

            return isCached;
        }

        public T GetItem(string videoId)
        {
            if (!_list.ContainsKey(videoId))
                return default(T);

            try
            {
                var tempQueue = _idOrderQueue.Where(i => i != videoId).Reverse().ToList();
                _idOrderQueue.Clear();

                foreach (var i in tempQueue)
                    _idOrderQueue.Enqueue(i);

                _idOrderQueue.Enqueue(videoId);
            }
            catch (Exception ex)
            {
                Log.Error($"YTII.{nameof(YouTubeModelCache)}.{nameof(GetItem)}.MoveIdToBottom", ex.Message);
            }

            Log.Info($"YTII.{nameof(YouTubeModelCache)}.{nameof(GetItem)}", $"Found Cached Video Item");
            return _list[videoId];
        }


    }
}