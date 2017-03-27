using System.Drawing;

namespace Conductor.GUI
{
    public interface ICartesianMapper
    {
        int Capacity { get; }
        int ColCount { get; set; }
        int RowCount { get; set; }
        Mapping.FillOrder Order { get; set; }
        string GetAddressFromCoordinates(int ColIndex, int RowIndex);
        string GetAddressFromOrdinal(int Ordinal);
        Point GetCoordinatesFromAddress(string p);
        Point GetCoordinatesFromOrdinal(int Ordinal);
        int GetOrdinalFromAddress(string Address);
        int GetOrdinalFromCoordinates(int ColIndex, int RowIndex);
        bool IsValidAddess(string Address);
    }
}