namespace ImpossibleOdds.DataMapping
{
	using System;
	using ImpossibleOdds.DataMapping.Processors;

	public static class DataMapper
	{
		/// <summary>
		/// Map a source value to a data structure as described in the given mapping definition.
		/// </summary>
		/// <param name="source">Source value to be mapped.</param>
		/// <param name="mappingDefinition">Definition of how the source should be mapped.</param>
		/// <returns>A mapped data structure </returns>
		public static object MapToDataStructure(object source, IMappingDefinition mappingDefinition)
		{
			mappingDefinition.ThrowIfNull(nameof(mappingDefinition));

			if (mappingDefinition.ToDataStructureProcessors == null)
			{
				throw new DataMappingException(string.Format("No mapping processors defined in the mapping definition of type {0}.", mappingDefinition.GetType().Name));
			}

			object result = null;
			foreach (IMapToDataStructureProcessor processor in mappingDefinition.ToDataStructureProcessors)
			{
				if (processor.MapToDataStructure(source, out result))
				{
					return result;
				}
			}

			if (source == null)
			{
				throw new DataMappingException("Failed to process null value.");
			}
			else
			{
				throw new DataMappingException(string.Format("Failed to process value of type {0}.", source.GetType().Name));
			}
		}

		/// <summary>
		/// Map the given source object to an instance of the target type.
		/// </summary>
		/// <param name="targetType">The target instance type.</param>
		/// <param name="source">The source object to be mapped to the target.</param>
		/// <param name="mappingDefinition">Definition of how the source should be mapped to the target.</param>
		/// <returns>An instance of the target type if the mapping was successfull. Null otherwise.</returns>
		public static object MapFromDataStructure(Type targetType, object source, IMappingDefinition mappingDefinition)
		{
			targetType.ThrowIfNull(nameof(targetType));
			mappingDefinition.ThrowIfNull(nameof(mappingDefinition));
			if (mappingDefinition.FromDataStructureProcessors == null)
			{
				throw new DataMappingException("No unmapping processors are defined.");
			}

			object result = null;
			foreach (IMapFromDataStructureProcessor processor in mappingDefinition.FromDataStructureProcessors)
			{
				if (processor.MapFromDataStructure(targetType, source, out result))
				{
					return result;
				}
			}

			throw new DataMappingException(string.Format("Failed to map a source value to target type {0}.", targetType.Name));
		}

		/// <summary>
		/// Map the given source object to the target instance.
		/// </summary>
		/// <param name="target">Target object unto which the source values will get mapped.</param>
		/// <param name="source">Source values that will get mapped to the target.</param>
		/// <param name="mappingDefinition">Definition of how the source should get mapped to the target.</param>
		public static void MapFromDataStructure(object target, object source, IMappingDefinition mappingDefinition)
		{
			target.ThrowIfNull(nameof(target));
			mappingDefinition.ThrowIfNull(nameof(target));

			foreach (IMapFromDataStructureProcessor processor in mappingDefinition.FromDataStructureProcessors)
			{
				if ((processor == null) || !(processor is IMapFromDataStructureToTargetProcessor))
				{
					continue;
				}

				// If the processor implements this interface, and was able to successfully map, then we can stop.
				if ((processor as IMapFromDataStructureToTargetProcessor).MapFromDataStructure(target, source))
				{
					return;
				}
			}

			throw new DataMappingException(string.Format("Failed to map a source value to a target instande of type {0}.", target.GetType().Name));
		}
	}
}
