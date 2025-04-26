using backend.Controllers;
using backend.Models;
using backend.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend_test
{
    public class BookingControllerTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private ClaimsPrincipal GetFakeUser(string userId = "test-user")
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));
        }

        [Fact]
        public async Task BookAppointment_ReturnsSuccess_WhenSlotIsAvailable()
        {
            // Arrange
            var db = GetDbContext();
            var controller = new BookingController(db);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = GetFakeUser() }
            };

            // Setup availability
            db.MechanicAvailabilities.Add(new MechanicAvailability
            {
                ApplicationUserId = "mechanic1",
                DayOfWeek = DateTime.Today.DayOfWeek,
                StartTime = TimeSpan.FromHours(8),
                EndTime = TimeSpan.FromHours(17)
            });
            await db.SaveChangesAsync();

            var time = DateTime.Today.AddHours(9);
            var model = new BookingViewModel
            {
                SelectedDate = time.Date,
                SelectedSlot = "09:00",
                Notes = "Test",
                SelectedServiceIds = new List<int>()
            };

            // Act
            var result = await controller.Book(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("BookingResponse", viewResult.ViewName);
        }

    }
}