using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Scripts.Objects;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    class MapManager
    {
        private List<KeyValuePair<int, GameObject>> _prefabs;
        
        private GameObject _mapContentsGroup;

        public MapManager()
        {
            _mapContentsGroup = GameObject.Find("MapContents");
            LoadPrefabs();
        }

        public MapManager(GameObject group)
        {
            _mapContentsGroup = group;
            LoadPrefabs();
        }

        private void LoadPrefabs()
        {
            _prefabs = new List<KeyValuePair<int, GameObject>>();

            TileObject[] tos = Resources.FindObjectsOfTypeAll<TileObject>();
            foreach (var to in tos)
            {
                GameObject go = to.gameObject;
                if (!go.activeInHierarchy && go.activeSelf)
                {
                    _prefabs.Add(new KeyValuePair<int, GameObject>(to.Id, go));
                }
            }

            Debug.Log("Count of loaded prefabs: " + _prefabs.Count);
            bool collisions = CheckForCollisions();

            if (collisions)
            {
                foreach (var prefab in _prefabs)
                {
                    Debug.Log(prefab.Value.name + ": " + prefab.Key);
                }
            }
        }

        private bool CheckForCollisions()
        {
            bool flag = false;
            for (int i = 0; i < _prefabs.Count; i++)
            {
                for (int j = i + 1; j < _prefabs.Count; j++)
                {
                    if (_prefabs[i].Key == _prefabs[j].Key)
                    {
                        flag = true;
                        string msg = String.Format("Prefab {0} and prefab {1} have same ID: {2}", _prefabs[i].Value.name, _prefabs[j].Value.name, _prefabs[i].Key);
                        SendError(msg);
                    }
                }
            }

            return flag;
        }

        private void SendError(string error)
        {
            Debug.Log(error);
        }

        public GameObject FindPrefabById(int id)
        {
            for (int i = 0; i < _prefabs.Count; i++)
            {
                if (_prefabs[i].Key == id)
                    return _prefabs[i].Value;
            }

            SendError("Prefab with ID " + id + " does not exist!");

            return null;
        }

        public void SaveScene(string mapName)
        {
            TileObject[] tos = GameObject.FindObjectsOfType<TileObject>();

            string mapFileContents = "";

            foreach (var to in tos)
            {
                if(!to.gameObject.activeInHierarchy)
                    continue;
                
                mapFileContents += "{\n\t";
                mapFileContents += to.Id + "\n\t";
                string mapData = to.ToMap();
                mapFileContents += mapData;
                mapFileContents += "\n}\n\n";
            }

            File.WriteAllText(mapName, mapFileContents);
        }

        public bool LoadMapToScene(string mapname)
        {

            bool ok = true;

            if (!File.Exists(mapname))
            {
                SendError("Map with name " + mapname + " does not exist!");
                return false;
            }

            string mapFileContents = File.ReadAllText(mapname);
            
            // Ищем очередную открывающую фигурную скобку
            // Пропускаем все пробелы и табуляции
            // Получаем ID префаба
            // Пропускаем все пробелы и табуляции
            // Считываем всё до закрывающей скобки
            // Создаем на сцене префаб по указанному ID
            // Посылаем ему считанную строку

            for (int i = 0; i < mapFileContents.Length; i++)
            {
                // Skipping possible comments
                while (i < mapFileContents.Length && mapFileContents[i] != '{')
                    i++;

                i++;

                if (i >= mapFileContents.Length)
                {
                    SendError("EOF reached!");
                    return ok;
                }

                // Skipping spaces, EOL and tabs
                while (i < mapFileContents.Length && (mapFileContents[i] == ' ' || mapFileContents[i] == '\t' || mapFileContents[i] == '\n'))
                    i++;

                if (i >= mapFileContents.Length)
                {
                    SendError("EOF reached!");
                    return ok;
                }

                // Reading ID
                int id;
                string idText = "";

                while (i < mapFileContents.Length && mapFileContents[i] != '\n' && mapFileContents[i] != ' ')
                {
                    idText += mapFileContents[i];
                    i++;
                }

                if (i >= mapFileContents.Length)
                {
                    SendError("EOF reached!");
                    return ok;
                }

                bool idParsed = Int32.TryParse(idText, out id);

                if (!idParsed)
                {
                    SendError("Impossible to parse ID: " + idText);
                    return false;
                }

                // Skipping spaces and tabs
                while (i < mapFileContents.Length && (mapFileContents[i] == ' ' || mapFileContents[i] == '\t'))
                    i++;

                // Reading object preferences
                string content = "";

                while (i < mapFileContents.Length && mapFileContents[i] != '}')
                {
                    content += mapFileContents[i];
                    i++;
                }

                content = content.Replace("\t", "");

                if (!SpawnObjectFromMap(id, content))
                {
                    ok = false;
                }
            }

            return ok;
        }

        private bool SpawnObjectFromMap(int id, string data)
        {
            GameObject prefab = FindPrefabById(id);

            if (prefab == null)
            {
                SendError("Prefab with ID " + id + " was not found!");
                return false;
            }

            GameObject intance = GameObject.Instantiate(prefab);

            if (_mapContentsGroup)
            {
                intance.transform.parent = _mapContentsGroup.transform;
            }

            bool ok = intance.GetComponent<TileObject>().FromMap(data);

            if (ok)
            {
                NetworkServer.Spawn(intance);
                //SendError(String.Format("Object (id: {0}, name: {1}, cell: [{2}, {3}]) spawned", id, prefab.name,
                    //intance.GetComponent<TileObject>().Cell.x,
                    //intance.GetComponent<TileObject>().Cell.y));
            }
            else
            {
                SendError("TileObject rejected this data!");
            }

            return ok;
        }
    }
}
