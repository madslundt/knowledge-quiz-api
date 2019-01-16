using AutoFixture;
using FluentAssertions;
using FluentValidation;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnitTest.Common;
using Xunit;

namespace UnitTest.Features.Locale
{
    public class GetLocaleTest : TestBase
    {
        [Fact]
        public async Task ThrowArgumentNullExceptionWhenLocaleIsNull()
        {
            var query = (API.Features.Locale.GetLocales.Query)null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ReturnListOfLocalesAvailable()
        {
            var count = Enum.GetNames(typeof(DataModel.Models.Localization.Locale)).Length;

            var locales = Enumerable.Range(0, count)
                .Select(x => _fixture.Build<DataModel.Models.Localization.LocaleReference>()
                            .Create())
                .ToList();

            _db.AddRange(locales);
            _db.SaveChanges();

            var result = await _mediator.Send(new API.Features.Locale.GetLocales.Query());

            result.Should().NotBeNull();
            result.Locales.Count.Should().Be(count);

            foreach (var locale in result.Locales)
            {
                locale.Id.Should().NotBeNull();

                locale.Name.Should().NotBeNull();
                locale.Name.Should().NotBeEmpty();

                locale.Code.Should().NotBeNull();
                locale.Code.Should().NotBeEmpty();
            }
        }

        [Fact]
        public async Task ReturnEmptyListWhenNoLocalesAreAvailable()
        {
            var result = await _mediator.Send(new API.Features.Locale.GetLocales.Query());

            result.Should().NotBeNull();
            result.Locales.Count.Should().Be(0);
        }
    }
}
