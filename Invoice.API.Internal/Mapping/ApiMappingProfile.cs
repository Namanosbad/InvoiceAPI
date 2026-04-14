using AutoMapper;
using Invoice.API.Domain.Entities;
using Invoice.API.Internal.Contracts.Responses;

namespace Invoice.API.Internal.Mapping;

public sealed class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<Client, ClientResponse>();

        CreateMap<ServiceItem, ServiceItemResponse>();

        CreateMap<InvoiceItem, InvoiceItemResponse>();

        CreateMap<Invoices, InvoiceResponse>()
            .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client != null ? src.Client.Name : null))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.InvoiceItems));
    }
}
