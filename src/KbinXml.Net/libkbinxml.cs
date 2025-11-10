#if !NETSTANDARD2_0 && !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace KbinXml.Net;

#pragma warning disable CS8981 // 该类型名称仅包含小写 ascii 字符。此类名称可能会成为该语言的保留值。
#pragma warning disable IDE1006 // 命名样式
internal unsafe class libkbinxml
#pragma warning restore IDE1006 // 命名样式
#pragma warning restore CS8981 // 该类型名称仅包含小写 ascii 字符。此类名称可能会成为该语言的保留值。
{
    /// <summary>
    /// C API 入口点：用于将 C 输入转换为 .NET 类型。
    /// </summary>
    [UnmanagedCallersOnly(EntryPoint = "Write")]
    public static int Write(
        byte* xmlBytesPtr,         // 输入: XML 数据的指针
        int xmlBytesLength,        // 输入: XML 数据的长度
        int encoding,              // 输入: KnownEncodings (作为 int)
        WriteOptionsStruct options, // 输入: C 兼容的选项
        byte** outputBytesPtr,     // 输出: 指向结果缓冲区的指针
        int* outputBytesLength)    // 输出: 结果缓冲区的长度
    {
        // 默认设置 "out" 参数为 null/0
        *outputBytesPtr = null;
        *outputBytesLength = 0;

        try
        {
            // 1. 将 C 输入转换为 .NET 类型
            var xmlSpan = new ReadOnlySpan<byte>(xmlBytesPtr, xmlBytesLength);
            var xmlBytes = xmlSpan.ToArray(); // 复制到托管数组

            var knownEncoding = (KnownEncodings)encoding;

            // 转换结构体
            var writeOptions = new WriteOptions
            {
                StrictMode = options.StrictMode,
                Compress = options.Compress,
                // 假设 RepairedPrefix 是 UTF-8 编码
                RepairedPrefix = (options.RepairedPrefix == IntPtr.Zero)
                    ? null
                    : Marshal.PtrToStringUTF8(options.RepairedPrefix),
                ZeroFillGap = options.ZeroFillGap
            };

            // 2. 调用你的 C# 核心逻辑
            byte[] resultBytes = KbinConverter.Write(xmlBytes, knownEncoding, writeOptions);

            if (resultBytes == null || resultBytes.Length == 0)
            {
                return 0; // 成功，但没有输出
            }

            // 3. 将 .NET 结果 (byte[]) 转换为 C 输出 (byte*)
            // 分配 *非托管* 内存
            var unmanagedBuffer = (byte*)NativeMemory.Alloc((nuint)resultBytes.Length);

            // 将数据从托管数组复制到非托管内存
            var resultSpan = new Span<byte>(unmanagedBuffer, resultBytes.Length);
            resultBytes.AsSpan().CopyTo(resultSpan);

            // 4. 设置 'out' 指针
            *outputBytesPtr = unmanagedBuffer;
            *outputBytesLength = resultBytes.Length;

            return 0; // 0 = 成功
        }
        catch (Exception)
        {
            // 在 C API 边界捕获所有异常
            // 你应该在这里记录日志
            return -1; // -1 = 失败
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "WriteText")]
    public static int WriteText(
        byte* xmlTextUtf8Ptr, // 输入: XML 文本的 UTF-8 指针
        int xmlTextUtf8Length, // 输入: XML 文本的 UTF-8 长度
        int encoding, // 输入: KnownEncodings (作为 int)
        IntPtr optionsPtr, // 输入: 指向 WriteOptionsStruct_Native 的指针, 或 IntPtr.Zero (null)
        byte** outputBytesPtr, // 输出: 指向结果缓冲区的指针
        int* outputBytesLength) // 输出: 结果缓冲区的长度
    {
        // 默认设置 "out" 参数为 null/0
        *outputBytesPtr = null;
        *outputBytesLength = 0;

        try
        {
            // 1. 将 C 输入 (UTF-8 指针+长度) 转换为 .NET string
            string xmlText = Encoding.UTF8.GetString(xmlTextUtf8Ptr, xmlTextUtf8Length);

            var knownEncoding = (KnownEncodings)encoding;

            // 2. 将 C 输入 (IntPtr) 转换为 .NET WriteOptionsStruct?
            WriteOptions? writeOptions = null;
            if (optionsPtr != IntPtr.Zero)
            {
                // 从指针读取 C 结构体
                var nativeOptions = Marshal.PtrToStructure<WriteOptionsStruct>(optionsPtr);

                // 转换 C 结构体为 .NET 结构体
                writeOptions = new WriteOptions
                {
                    StrictMode = nativeOptions.StrictMode,
                    Compress = nativeOptions.Compress,
                    RepairedPrefix = (nativeOptions.RepairedPrefix == IntPtr.Zero)
                        ? null
                        : Marshal.PtrToStringUTF8(nativeOptions.RepairedPrefix),
                    ZeroFillGap = nativeOptions.ZeroFillGap
                };
            }

            // 3. 调用你的 C# 核心逻辑
            byte[] resultBytes = KbinConverter.Write(xmlText, knownEncoding, writeOptions);

            if (resultBytes == null || resultBytes.Length == 0)
            {
                return 0; // 成功，但没有输出
            }

            // 4. 将 .NET 结果 (byte[]) 转换为 C 输出 (byte*)
            var unmanagedBuffer = (byte*)NativeMemory.Alloc((nuint)resultBytes.Length);
            var resultSpan = new Span<byte>(unmanagedBuffer, resultBytes.Length);
            resultBytes.AsSpan().CopyTo(resultSpan);

            // 5. 设置 'out' 指针
            *outputBytesPtr = unmanagedBuffer;
            *outputBytesLength = resultBytes.Length;

            return 0; // 0 = 成功
        }
        catch (Exception ex)
        {
            // 在 C API 边界捕获所有异常
            // 你应该在这里记录日志
            Console.Error.WriteLine($"[C# Error] {ex.Message}");
            return -1; // -1 = 失败
        }
    }

    /// <summary>
    /// 导出一个 "Free" 函数，供 C 调用者释放内存
    /// </summary>
    [UnmanagedCallersOnly(EntryPoint = "FreeMemory")]
    public static void FreeMemory(byte* buffer)
    {
        NativeMemory.Free(buffer);
    }
}

internal struct WriteOptionsStruct
{
    public bool StrictMode;
    public bool Compress;
    public IntPtr RepairedPrefix;
    public bool ZeroFillGap;

    public WriteOptionsStruct()
    {
        StrictMode = true;
        Compress = true;
        RepairedPrefix = IntPtr.Zero;
        ZeroFillGap = true;
    }
}

#endif