using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Frankcrum.LargeCheck.Infrastructure;
using Frankcrum.LargeCheck.Applications.Interfaces.Repositories;
using Frankcrum.LargeCheck.Applications.Entities;
using Frankcrum.LargeCheck.Infrastructure.Repositories;

namespace Frankcrum.LargeCheck.Repository.Test.Repository
{
    [TestClass]
    public class LargeCheckRepositoryTest
    {
        private static Mock<ILargeCheckRepository> _largeCheckProcessorStub;
        private readonly List<DeductionChangesRequestResponse> _largecheckList;
        private readonly List<CorporatePayGroups> _paygroupsList;
        public LargeCheckRepositoryTest()
        {
            _largeCheckProcessorStub = new Mock<ILargeCheckRepository>();
            IEnumerable<DeductionChangesRequestResponse> largecheckResponse = new List<DeductionChangesRequestResponse>
                {
                    new DeductionChangesRequestResponse { NetAmt="100", DocNo="123", HourlyRate="23", TotalHours="34", EmpNo="7890",PayGroup="456"},
                    new DeductionChangesRequestResponse { NetAmt="110", DocNo="145", HourlyRate="20", TotalHours="56", EmpNo="8906",PayGroup="567"},
        };
            _largecheckList = largecheckResponse.ToList();


            IEnumerable<CorporatePayGroups> paygroupResponse = new List<CorporatePayGroups>
                {
                    new CorporatePayGroups { PayGroups="6Z006W"},
            };
            _paygroupsList = paygroupResponse.ToList();
        }


        [TestMethod]
        public void GetLargeCheckDailySummary()
        {
            _largeCheckProcessorStub.Setup(mr => mr.GetLargeCheckDailySummary(_paygroupsList)).ReturnsAsync(_largecheckList);
            var response = _largeCheckProcessorStub.Object.GetLargeCheckDailySummary(_paygroupsList).ConfigureAwait(false);
            Assert.IsNotNull(response);
        }
    }
}
