md UqsAppointmentBooking
cd UqsAppointmentBooking
dotnet new sln
dotnet new blazorwasm -n Uqs.AppointmentBooking.Website
dotnet new webapi -n Uqs.AppointmentBooking.WebApi
dotnet new classlib -n Uqs.AppointmentBooking.Contract
dotnet new classlib -n Uqs.AppointmentBooking.Domain
dotnet new xunit -n Uqs.AppointmentBooking.Domain.Tests.Unit
dotnet sln add Uqs.AppointmentBooking.Website
dotnet sln add Uqs.AppointmentBooking.WebApi
dotnet sln add Uqs.AppointmentBooking.Contract
dotnet sln add Uqs.AppointmentBooking.Domain
dotnet sln add Uqs.AppointmentBooking.Domain.Tests.Unit
dotnet add Uqs.AppointmentBooking.Website reference Uqs.AppointmentBooking.Contract
dotnet add Uqs.AppointmentBooking.WebApi reference Uqs.AppointmentBooking.Contract
dotnet add Uqs.AppointmentBooking.Domain reference Uqs.AppointmentBooking.Contract
dotnet add Uqs.AppointmentBooking.WebApi reference Uqs.AppointmentBooking.Domain
dotnet add Uqs.AppointmentBooking.Domain.Tests.Unit reference Uqs.AppointmentBooking.Domain
dotnet add Uqs.AppointmentBooking.Domain package Microsoft.EntityFrameworkCore.SqlServer
dotnet add Uqs.AppointmentBooking.Domain.Tests.Unit package NSubstitute
dotnet add Uqs.AppointmentBooking.Domain.Tests.Unit package Microsoft.EntityFrameworkCore.InMemory