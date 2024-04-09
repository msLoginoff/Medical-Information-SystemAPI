using MedicalInformationSystem.Exceptions;

namespace MedicalInformationSystem.Models;

public class PageInfoModel
{
    public int Size { get; set; } 
    public int Count { get; set; }
    public int Current { get; set; }
    public PageInfoModel(int size, int count, int current)
    {
        if (size == 0)
        {
            current = 0;
            size = 1;
        }
        
        Size = size;
        Current = current;
        Count = (count + size - 1) / size;
        
        if ((Current > Count && Count != 0) || Current < 1 || Size < 1)
        {
            throw new BadRequest("Invalid value for attribute page");
        }
    }
}