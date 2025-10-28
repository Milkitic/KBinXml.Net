# KbinXml.Net Samples

This project contains comprehensive examples demonstrating the usage of KbinXml.Net library. All code examples are based on the documentation in the main README file.

## Project Structure

- **Program.cs** - Main entry point with basic usage examples
- **AdvancedSamples.cs** - Advanced usage scenarios and performance examples
- **ConfigurationSamples.cs** - Configuration options and element name repair examples
- **data/** - Sample data files for testing

## Running the Samples

1. Build the project:
   ```bash
   dotnet build
   ```

2. Run the samples:
   ```bash
   dotnet run
   ```

## Sample Categories

### 1. Quick Start Samples
- Basic reading and writing operations
- Different XML format conversions
- File I/O operations

### 2. Configuration Options
- ReadOptions and WriteOptions usage
- Supported encodings demonstration
- Default vs custom settings

### 3. Advanced Usage
- Stream-based processing for large files
- Encoding comparisons
- Compression effectiveness
- Performance measurements
- Different XML API usage

### 4. Element Name Repair
- Handling invalid XML element names
- RepairedPrefix configuration
- Round-trip compatibility
- Validation scenarios

### 5. Error Handling
- KbinException handling
- Input validation
- Data format verification

## Key Features Demonstrated

- **Performance Optimization**: Stream processing and memory efficiency
- **Multi-framework Support**: Compatible with .NET 8.0 and .NET 9.0
- **Multiple XML APIs**: LINQ to XML, W3C DOM, and raw bytes
- **Encoding Support**: UTF-8, Shift-JIS, EUC-JP, and more
- **Compression**: SixBit compression for reduced file sizes
- **Memory Efficiency**: RecyclableMemoryStream usage
- **Thread Safety**: Safe for concurrent operations
- **Element Name Repair**: Automatic handling of invalid XML names

## Sample Data

The `data/` folder contains sample XML files that can be used for testing:
- `sample.xml` - Basic XML structure with various data types

## Notes

- All examples include proper error handling
- Performance measurements include warm-up iterations
- Stream-based examples demonstrate memory-efficient processing
- Configuration examples show both default and custom settings
- Element name repair examples simulate real-world scenarios

For more detailed information, refer to the main project README file.