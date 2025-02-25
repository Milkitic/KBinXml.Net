using System.Text;
using System.Xml.Linq;
using KbinXml.Net;
using Xunit;
using Xunit.Abstractions;

namespace GeneralUnitTests
{
    public class WritingAndReadingTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public WritingAndReadingTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [Theory]
        [InlineData(@"<root>
    <rarity __type=""u8"">5</rarity>
    <generator_no __type=""u16"">1</generator_no>
    <distribution_date __type=""u32"">12356134</distribution_date>
    <is_default __type=""u64"">21512976124353</is_default>
    <sort_no __type=""s8"">-121</sort_no>
    <genre __type=""s16"">-5126</genre>
    <limited __type=""s32"">-35721234</limited>
    <wtf_is_this __type=""s64"">-253178167252134</wtf_is_this>
</root>")]
        public void TestNumbers(string value)
        {
            DoWorks(value);
        }

        [Theory]
        [InlineData(@"<card id=""5502"">
    <info>
        <texture __type=""str"">ap_06_R0002</texture>
        <title __type=""str"">ボルテ10周年★記念限定カード</title>
        <message_a __type=""str"">はわわ～！ボルテはついニ[br:0]10周年を迎えまシタ～♪</message_a>
        <message_b __type=""str"">10周年…[br:0]大きな節目を迎えたわね！</message_b>
        <message_c __type=""str"">∩ ^-^ )∩わーい！[br:0]∩・v・)∩お祝いしよー！</message_c>
        <message_d __type=""str"">わわワ～！にぎゃかですッ！[br:0]全員集合ですョーッ！</message_d>
        <message_e __type=""str"">Foo！記念すべき日に[br:0]先生も燃えているゾッ★</message_e>
        <message_f __type=""str"">魂たちに連れまわされて[br:0]眠気が…、オヤスミ…。</message_f>
        <message_g __type=""str"">料理はまだまだあるよ！[br:0]おにーちゃん♪</message_g>
        <message_h __type=""str"">*△ これからもボルテを！[br:0]†△ 宜しくお願いします。</message_h>
    </info>
</card>")]
        public void WriteString(string value)
        {
            DoWorks(value);
        }

        [Theory]
        [InlineData(@"<music_list>
    <flag __type=""s32"" __count=""16"" sheet_type=""0"">21 52 11 53 43 134 21 -43 -12 -61 -13 -52 -47 -114 21 52 11 53 43 134 21 -43 -12 -61 -13 -52 -47 -114 134 21 -43 -12</flag>

</music_list>")]
        public void TestArrayNotValid(string value)
        {
            DoWorks(value);
        }

        [Theory]
        [InlineData(@"<music_list>
    <flag __type=""s32"" __count=""32"" sheet_type=""0"">-1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1</flag>
    <flag __type=""s32"" __count=""32"" sheet_type=""1"">-1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1</flag>
    <flag __type=""s32"" __count=""32"" sheet_type=""2"">-1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1</flag>
    <flag __type=""s32"" __count=""32"" sheet_type=""3"">-1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1</flag>
</music_list>")]
        public void TestArray(string value)
        {
            DoWorks(value);
        }

        [Theory]
        [InlineData(@"<kingdom>
    <cf __type=""bin"" __size=""16"">F34255041131EDDFC769181B8F33892E</cf>
    <qcf __type=""bin"" __size=""32"">AA4A965AA8C2C169D145E75B5DA93879CD8AD1A3F32185662DC54341263DBB03</qcf>
    <piece __type=""bin"" __size=""40"">1CE9481CA73F4B0AD6867EB0D51A0E1672946BE5B6D1B109F327348C9B7CBB2C15781A0482C3953C</piece>
</kingdom>")]
        public void TestBinary(string value)
        {
            DoWorks(value);
        }

        [Fact]
        public void ErrorCaseMid()
        {
            var text = GetMidText();
            DoWorks(text);
        }

        private string GetMidText()
        {
            return @"<response status=""0"" fault=""0"" dstid=""218D0B63818DD551C2BE"">
    <game status=""0"" fault=""0"">
        <param>
            <info>
                <param __type=""s32"" __count=""7"">1 1 1 1 1 1 </param>
            </info>
        </param>
    </game>
</response>";
        }

        private void DoWorks(string value)
        {
            var xml = XElement.Parse(value);

            var bytes2 = KbinConverter.Write(xml, KnownEncodings.UTF8, new WriteOptions() { StrictMode = false });
            var cvt = new StableKbin.XmlWriter(xml, Encoding.UTF8);
            var bytes = cvt.Write();

            var result2 = KbinConverter.ReadXmlLinq(bytes).ToString();
            var cvt2 = new StableKbin.XmlReader(bytes);
            var result = cvt2.ReadLinq().ToString();

            _outputHelper.WriteLine(result);
            _outputHelper.WriteLine(result2);
            Assert.Equal(bytes, bytes2);
            Assert.Equal(result, result2);
        }
    }
}
