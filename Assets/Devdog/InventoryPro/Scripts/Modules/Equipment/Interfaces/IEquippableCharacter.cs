using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;

namespace Devdog.InventoryPro
{
    public interface IEquippableCharacter : ICharacterStats
    {
        CharacterEquipmentTypeBinder[] equipmentBinders { get; set; }
        CharacterEquipmentHandlerBase equipmentHandler { get; }
    }
}
