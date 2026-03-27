using CRM.Shared.Application;
using CRM.Shared.Common;
using CRMService.Application.Commands.Contacts;
using CRMService.Application.Interfaces;
using CRMService.Domain.Entities;
using CRMService.Domain.Repositories;

namespace CRMService.Application.Handlers.Contacts;

internal sealed class CreateContactHandler : ICommandHandler<CreateContactCommand, Result<ContactDto>>
{
    private readonly IContactRepository _contacts;
    private readonly IUnitOfWork        _uow;

    public CreateContactHandler(IContactRepository contacts, IUnitOfWork uow) => (_contacts, _uow) = (contacts, uow);

    public async Task<Result<ContactDto>> Handle(CreateContactCommand req, CancellationToken ct)
    {
        if (await _contacts.EmailExistsAsync(req.Email, ct))
            return Error.Conflict with { Code = "Contact.Duplicate", Description = "A contact with this email already exists." };

        var contact = Contact.Create(req.FirstName, req.LastName, req.Email, req.OwnerId, req.Phone, req.Company, req.JobTitle);
        await _contacts.AddAsync(contact, ct);
        await _uow.SaveChangesAsync(ct);
        return contact.ToDto();
    }
}

internal sealed class UpdateContactHandler : ICommandHandler<UpdateContactCommand, Result<ContactDto>>
{
    private readonly IContactRepository _contacts;
    private readonly IUnitOfWork        _uow;

    public UpdateContactHandler(IContactRepository contacts, IUnitOfWork uow) => (_contacts, _uow) = (contacts, uow);

    public async Task<Result<ContactDto>> Handle(UpdateContactCommand req, CancellationToken ct)
    {
        var contact = await _contacts.GetByIdAsync(req.ContactId, ct);
        if (contact is null) return Error.NotFound with { Code = "Contact.NotFound" };

        contact.Update(req.FirstName, req.LastName, req.Email, req.Phone, req.Company, req.JobTitle);
        await _contacts.UpdateAsync(contact, ct);
        await _uow.SaveChangesAsync(ct);
        return contact.ToDto();
    }
}

internal sealed class DeleteContactHandler : ICommandHandler<DeleteContactCommand, Result>
{
    private readonly IContactRepository _contacts;
    private readonly IUnitOfWork        _uow;

    public DeleteContactHandler(IContactRepository contacts, IUnitOfWork uow) => (_contacts, _uow) = (contacts, uow);

    public async Task<Result> Handle(DeleteContactCommand req, CancellationToken ct)
    {
        var contact = await _contacts.GetByIdAsync(req.ContactId, ct);
        if (contact is null) return Error.NotFound with { Code = "Contact.NotFound" };
        await _contacts.DeleteAsync(contact, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
