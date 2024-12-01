namespace Shopipy.Shared.DTOs;

public class PaginationResultDto<T>
{
    public required IEnumerable<T> Data { get; set; }
    public required int Count { get; set; }
}