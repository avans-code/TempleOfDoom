using CODE_TempleOfDoom_DownloadableContent;

namespace TempleOfDoom.Domain.Enemies;

// Deze klasse voorkomt dat de DLL crasht door altijd een geldige vloer terug te geven
public class FieldAdapter : IField
{
    public bool CanEnter => true;
    public IPlacable Item { get; set; }

    // We returneren onszelf zodat de DLL nooit een NullReference krijgt als hij naar de buurman vraagt
    public IField GetNeighbour(int direction)
    {
        return this;
    }
}