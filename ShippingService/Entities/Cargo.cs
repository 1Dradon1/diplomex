using ShippingService.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShippingService.Entities;

public class Cargo : IItem
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Weight { get; set; }
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public int? StorageId { get; set; }
    [ForeignKey(nameof(TransportProtocol))]
    public int? TransportProtocolId { get; set; }

    public Storage? Storage { get; set; }
    public TransportProtocol? TransportProtocol { get; set; }
}