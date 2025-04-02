using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dotnet.Template.Infrastructure.Convertors;

public class GuidToStringConvertor(ConverterMappingHints? mappingHints = null) : ValueConverter<Guid, string>(x => x.ToString(), x => Guid.Parse(x), DefaultHints.With(mappingHints))
{
    private static readonly ConverterMappingHints DefaultHints = new(size: 26);

    /// <summary>
    /// Default instance
    /// </summary>
    public static readonly GuidToStringConvertor Instance = new();
}

