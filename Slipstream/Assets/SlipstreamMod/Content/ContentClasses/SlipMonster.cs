using MSU;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Slipstream
{
    public abstract class SlipMonster : IMonsterContentPiece
    {
        public abstract NullableRef<MonsterCardProvider> CardProvider { get; }
        public abstract NullableRef<DirectorAPI.DirectorCardHolder> DissonanceCard { get; }
        public abstract NullableRef<GameObject> MasterPrefab { get; }
        CharacterBody IGameObjectContentPiece<CharacterBody>.Component => CharacterPrefab.GetComponent<CharacterBody>();
        GameObject IContentPiece<GameObject>.Asset => CharacterPrefab;
        public abstract GameObject CharacterPrefab { get; }

        public abstract void Initialize();
        public abstract bool IsAvailable(ContentPack contentPack);
        public abstract IEnumerator LoadContentAsync();
    }
}