using NUnit.Framework;

namespace GreenCarWashApp.Tests
{
    [SetUpFixture]
    public class TestRunner
    {
        [OneTimeSetUp]
        public void GlobalSetup()
        {
            // Global test setup - runs once before all tests
            Console.WriteLine("Starting Green Car Wash App Test Suite");
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            // Global test cleanup - runs once after all tests
            Console.WriteLine("Green Car Wash App Test Suite Completed");
        }
    }
}