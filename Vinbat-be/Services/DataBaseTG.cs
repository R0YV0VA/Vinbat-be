using Microsoft.EntityFrameworkCore;
using Vinbat_be.Contexts;
using Vinbat_be.Models;

namespace Vinbat_be.Services;

public class DataBaseTG
{
    private readonly CasesContext casesContext;
    public DataBaseTG(CasesContext _casesContext)
    {
        this.casesContext = _casesContext;
    }

    public async Task<List<Case>> GetAllCases()
    {
        return await casesContext.Cases.ToListAsync();
    }

    public async Task<Case> GetCase(int id)
    {
        return await casesContext.Cases.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Case> CloseOpenCase(int id) 
    {
        Case caseToClose = await casesContext.Cases.FirstOrDefaultAsync(c => c.Id == id);
        if (caseToClose == null)
            return null;
        caseToClose.Status = !caseToClose.Status;
        await casesContext.SaveChangesAsync();
        return caseToClose;
    }

    public async Task<Case> DeleteCase(int id)
    {
       Case caseToDelete = await casesContext.Cases.FirstOrDefaultAsync(c => c.Id == id);
       casesContext.Cases.Remove(caseToDelete);
       await casesContext.SaveChangesAsync();
       return caseToDelete;
    }
}
