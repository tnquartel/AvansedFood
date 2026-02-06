using Application.Services;
using Domain.Models;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace AvansedFood.Tests.Services
{
    public class StudentServiceTests
    {
        private readonly Mock<IStudentRepository> _studentRepositoryMock;
        private readonly Mock<IPackageRepository> _packageRepositoryMock;
        private readonly StudentService _studentService;

        public StudentServiceTests()
        {
            _studentRepositoryMock = new Mock<IStudentRepository>();
            _packageRepositoryMock = new Mock<IPackageRepository>();
            _studentService = new StudentService(_studentRepositoryMock.Object, _packageRepositoryMock.Object);
        }

        [Fact]
        public void RegisterStudent_Under16_ShouldReturnError()
        {
            // Arrange
            var youngStudent = new Student
            {
                Name = "Too Young",
                Email = "young@avans.nl",
                BirthDate = DateTime.Now.AddYears(-15),
                StudentNumber = "123456",
                PhoneNumber = "0612345678",
                StudyCity = City.Breda
            };

            // Act
            var result = _studentService.RegisterStudent(youngStudent);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("minimaal 16");
        }

        [Fact]
        public void RegisterStudent_EmailAlreadyExists_ShouldReturnError()
        {
            // Arrange
            var existingEmail = "existing@avans.nl";
            var existingStudent = new Student
            {
                StudentId = 1,
                Name = "Existing Student",
                Email = existingEmail,
                BirthDate = new DateTime(2000, 1, 1),
                StudentNumber = "111111",
                PhoneNumber = "0612345678",
                StudyCity = City.Breda
            };

            var newStudent = new Student
            {
                Name = "New Student",
                Email = existingEmail,
                BirthDate = new DateTime(2001, 1, 1),
                StudentNumber = "222222",
                PhoneNumber = "0687654321",
                StudyCity = City.Tilburg
            };

            _studentRepositoryMock
                .Setup(x => x.GetByEmail(existingEmail))
                .Returns(existingStudent);

            // Act
            var result = _studentService.RegisterStudent(newStudent);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("al geregistreerd");
        }

        [Fact]
        public void RegisterStudent_StudentNumberAlreadyExists_ShouldReturnError()
        {
            // Arrange
            var existingStudentNumber = "123456";
            var existingStudent = new Student
            {
                StudentId = 1,
                Name = "Existing Student",
                Email = "existing@avans.nl",
                BirthDate = new DateTime(2000, 1, 1),
                StudentNumber = existingStudentNumber,
                PhoneNumber = "0612345678",
                StudyCity = City.Breda
            };

            var newStudent = new Student
            {
                Name = "New Student",
                Email = "new@avans.nl",
                BirthDate = new DateTime(2001, 1, 1),
                StudentNumber = existingStudentNumber,
                PhoneNumber = "0687654321",
                StudyCity = City.Tilburg
            };

            _studentRepositoryMock
                .Setup(x => x.GetByStudentNumber(existingStudentNumber))
                .Returns(existingStudent);

            // Act
            var result = _studentService.RegisterStudent(newStudent);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("al geregistreerd");
        }

        [Fact]
        public void RegisterStudent_BirthdateInFuture_ShouldReturnError()
        {
            // Arrange
            var futureStudent = new Student
            {
                Name = "Future Student",
                Email = "future@avans.nl",
                BirthDate = DateTime.Now.AddDays(1),
                StudentNumber = "123456",
                PhoneNumber = "0612345678",
                StudyCity = City.Breda
            };

            // Act
            var result = _studentService.RegisterStudent(futureStudent);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("minimaal 16");
        }

        [Fact]
        public void RegisterStudent_ValidStudent_ShouldSucceed()
        {
            // Arrange
            var validStudent = new Student
            {
                Name = "Valid Student",
                Email = "valid@avans.nl",
                BirthDate = new DateTime(2000, 1, 1),
                StudentNumber = "123456",
                PhoneNumber = "0612345678",
                StudyCity = City.Breda
            };

            _studentRepositoryMock.Setup(x => x.GetByEmail(validStudent.Email)).Returns((Student?)null);
            _studentRepositoryMock.Setup(x => x.GetByStudentNumber(validStudent.StudentNumber)).Returns((Student?)null);

            // Act
            var result = _studentService.RegisterStudent(validStudent);

            // Assert
            result.Success.Should().BeTrue();
            _studentRepositoryMock.Verify(x => x.Add(validStudent), Times.Once);
        }
    }
}