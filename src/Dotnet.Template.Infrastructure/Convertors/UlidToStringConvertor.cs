using System;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dotnet.Template.Infrastructure.Convertors;

public class UlidToStringConvertor(ConverterMappingHints? mappingHints = null) : ValueConverter<Ulid, string>(x => x.ToString(), x => Ulid.Parse(x), DefaultHints.With(mappingHints))
{
    private static readonly ConverterMappingHints DefaultHints = new(size: 26);

    /// <summary>
    /// Default instance
    /// </summary>
    public static readonly UlidToStringConvertor Instance = new();
}

