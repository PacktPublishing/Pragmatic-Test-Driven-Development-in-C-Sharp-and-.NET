using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Uqs.AppointmentBooking.Domain.DomainObjects;

namespace Uqs.AppointmentBooking.Domain.Tests.Unit.Fakes;

public class ApplicationContextFakeBuilder : IDisposable
{
    private readonly ApplicationContextFake _context = new();
    
    private EntityEntry<Employee> _tomEmp;
    private EntityEntry<Employee> _janeEmp;
    private EntityEntry<Employee> _willEmp;
    private EntityEntry<Employee> _jessEmp;
    private EntityEntry<Employee> _edEmp;
    private EntityEntry<Employee> _oliEmp;

    public ApplicationContextFakeBuilder WithSingleEmployeeTom()
    {
        _tomEmp = _context.Add(new Employee { Name = "Thomas Fringe" });
        return this;
    }

    public ApplicationContextFakeBuilder WithEmployees()
    {
        _tomEmp = _context.Add(new Employee { Name = "Thomas Fringe" });
        _janeEmp = _context.Add(new Employee { Name = "Jane Haircomb" });
        _willEmp = _context.Add(new Employee { Name = "William Scissors" });
        _jessEmp = _context.Add(new Employee { Name = "Jessica Clipper" });
        _edEmp = _context.Add(new Employee { Name = "Edward Sideburn" });
        _oliEmp = _context.Add(new Employee { Name = "Oliver Bold" });
        return this;
    }

    public ApplicationContextFakeBuilder WithSingleShiftForTom(DateTime from, DateTime to)
    {
        _context.Add(new Shift { Employee = _tomEmp.Entity, Starting = from, Ending = to });
        return this;
    }

    public ApplicationContextFakeBuilder WithSingle30MinService()
    {
        var mensCut = _context.Add(new Service
        { Name = "Men's Cut", AppointmentTimeSpanInMin = 30, Price = 23, IsActive = true });

        return this;
    }

    public ApplicationContextFakeBuilder WithServices()
    {
        var mensCut = _context.Add(new Service
        { Name = "Men's Cut", AppointmentTimeSpanInMin = 30, Price = 23, IsActive = true });
        var mensClipperScissor = _context.Add(new Service
        { Name = "Men - Clipper & Scissor Cut", AppointmentTimeSpanInMin = 30, Price = 23, IsActive = true });
        var mensBeardTrim = _context.Add(new Service
        { Name = "Men - Beard Trim", AppointmentTimeSpanInMin = 10, Price = 10, IsActive = true });
        var mensColoring = _context.Add(new Service
        { Name = "Men - Full Head Coloring", AppointmentTimeSpanInMin = 70, Price = 60, IsActive = true });
        var mensPerm = _context.Add(new Service
        { Name = "Men - Perm", AppointmentTimeSpanInMin = 100, Price = 90, IsActive = true });
        var mensKeratin = _context.Add(new Service
        { Name = "Men - Keratin Treatment", AppointmentTimeSpanInMin = 120, Price = 100, IsActive = false });
        var boysCut = _context.Add(new Service
        { Name = "Boys - Cut", AppointmentTimeSpanInMin = 30, Price = 15, IsActive = true });
        var girlsCut = _context.Add(new Service
        { Name = "Girls - Cut", AppointmentTimeSpanInMin = 30, Price = 17, IsActive = true });

        return this;
    }

    public ApplicationContextFake Build()
    {
        _context.SaveChanges();
        return _context;
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}