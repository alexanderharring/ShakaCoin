using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelDB;

namespace ShakaCoin.Blockchain
{
    internal class DatabaseInteraction
    {
        private Options _options = new Options { CreateIfMissing = true };
        private DB _database;

        internal DatabaseInteraction(string DBPath)
        {
            _database = new DB(_options, DBPath);
        }

        internal void AddValue(byte[] key, byte[] value)
        {
            _database.Put(key, value);

        }

        internal byte[] GetValue(byte[] key)
        {
            return _database.Get(key);
        }

        internal void RemoveValue(byte[] key)
        {
            _database.Delete(key);
        }

        internal void Close()
        {
            _database.Close();
        }
    }
}
