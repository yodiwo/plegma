using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    public class MqttTopicNode<TKey>
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public string TopicNodeId;
        //------------------------------------------------------------------------------------------------------------------------
        public MqttTopicNode<TKey> ParentNode;
        //------------------------------------------------------------------------------------------------------------------------
        public DictionaryTS<string, MqttTopicNode<TKey>> Children = new DictionaryTS<string, MqttTopicNode<TKey>>();
        //------------------------------------------------------------------------------------------------------------------------
        public HashSetTS<TKey> Keys = new HashSetTS<TKey>();
        public HashSetTS<TKey> PromiscuousKeys = new HashSetTS<TKey>();
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void Add(string topic, TKey key)
        {
            //check
            if (string.IsNullOrWhiteSpace(topic))
                return;

            //break topic apart
            var elements = splitTopic(topic);
            _recAdd(elements, key, 0);
        }
        //------------------------------------------------------------------------------------------------------------------------
        void _recAdd(string[] elements, TKey key, int index)
        {
            //check
            if (elements == null || elements.Length == 0)
                return;

            //check if index is last element
            if (index >= elements.Length)
            {
                //add to keys
                Keys.Add(key);
            }
            else if (index == elements.Length - 1 && (elements[index] == "#" || elements[index] == "+"))
            {
                //add to promiscious keys
                PromiscuousKeys.Add(key);
            }
            else
            {
                //add on proper list
                MqttTopicNode<TKey> node;
                lock (Children)
                {
                    node = Children.TryGetOrDefault(elements[index]);
                    if (node == null)
                        Children.Add(elements[index], node = new MqttTopicNode<TKey>() { ParentNode = this, TopicNodeId = elements[index] });
                }
                //continue recursion
                node._recAdd(elements, key, index + 1);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool Remove(string topic, TKey key)
        {
            //check
            if (string.IsNullOrWhiteSpace(topic))
                return false;

            //break topic apart
            var elements = splitTopic(topic);
            return _recRemove(elements, key, 0);
        }
        //------------------------------------------------------------------------------------------------------------------------
        bool _recRemove(string[] elements, TKey key, int index)
        {
            //check
            if (elements == null || elements.Length == 0)
                return false;

            //check if index is last element
            bool removed = false;
            if (index >= elements.Length)
            {
                //add to keys
                Keys.Remove(key);
                //remove me from parent
                if (Keys.Count == 0)
                    lock (Children)
                    {
                        var me = ParentNode.Children.TryGetOrDefault(TopicNodeId);
                        if (this == me)
                            ParentNode.Children.Remove(TopicNodeId);
                    }
            }
            else if (index == elements.Length - 1 && (elements[index] == "#" || elements[index] == "+"))
            {
                //add to promiscious keys
                PromiscuousKeys.Remove(key);
            }
            else
            {
                //add on proper list
                MqttTopicNode<TKey> node;
                lock (Children)
                {
                    node = Children.TryGetOrDefault(elements[index]);
                    if (node == null)
                        return false;
                }
                //continue recursion
                removed = node._recRemove(elements, key, index + 1);
            }

            //cleanup?
            return removed | cleanUp();
        }
        //------------------------------------------------------------------------------------------------------------------------
        bool cleanUp()
        {
            lock (Children)
            {
                if (ParentNode != null && Children.Count == 0 && Keys.Count == 0 && PromiscuousKeys.Count == 0)
                {
                    var me = ParentNode.Children.TryGetOrDefault(TopicNodeId);
                    if (this == me)
                        return ParentNode.Children.Remove(TopicNodeId);
                }
            }
            //not removed
            return false;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public ISet<TKey> GetKeysSet(string topic)
        {
            //check
            if (string.IsNullOrWhiteSpace(topic))
                return new HashSet<TKey>();

            //break topic apart
            var elements = splitTopic(topic);

            //recursive find
            return _GetKeys(elements, 0).ToHashSet();
        }
        //------------------------------------------------------------------------------------------------------------------------
        public IEnumerable<TKey> GetKeys(string topic)
        {
            //check
            if (string.IsNullOrWhiteSpace(topic))
                return new List<TKey>();

            //break topic apart
            var elements = splitTopic(topic);

            //recursive find
            return _GetKeys(elements, 0);
        }
        //------------------------------------------------------------------------------------------------------------------------
        IEnumerable<TKey> _GetKeys(string[] elements, int index)
        {
            //check
            if (elements == null || elements.Length == 0)
                yield break;

            //return all promiscious keys
            if (PromiscuousKeys.Count > 0)
                foreach (var entry in PromiscuousKeys)
                    yield return entry;

            //check if index is last element
            if (index >= elements.Length)
            {
                //return node keys
                if (Keys.Count > 0)
                    foreach (var entry in Keys)
                        yield return entry;
            }
            else
            {
                //return all whildcar keys
                var wildchar = Children.TryGetOrDefault("+");
                if (wildchar != null)
                    foreach (var entry in wildchar._GetKeys(elements, index + 1))
                        yield return entry;

                //find next node
                var node = Children.TryGetOrDefault(elements[index]);
                if (node != null)
                    foreach (var entry in node._GetKeys(elements, index + 1))
                        yield return entry;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override string ToString()
        {
            return "TopicNode : " + TopicNodeId;
        }
        //------------------------------------------------------------------------------------------------------------------------
        string[] splitTopic(string topic)
        {
            //check
            if (string.IsNullOrWhiteSpace(topic))
                return null;

            //pre-process
            topic = topic.Trim();
            if (topic.EndsWith("/")) topic = topic.RemoveLast();

            //split it
            return topic.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
