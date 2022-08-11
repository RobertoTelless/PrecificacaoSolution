using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IContaBancariaRepository : IRepositoryBase<CONTA_BANCO>
    {
        CONTA_BANCO GetItemById(Int32 id);
        List<CONTA_BANCO> GetAllItens(Int32 idAss);
    }
}

