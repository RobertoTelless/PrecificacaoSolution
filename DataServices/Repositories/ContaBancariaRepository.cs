using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Data.Entity;
using EntitiesServices.Work_Classes;

namespace DataServices.Repositories
{
    public class ContaBancariaRepository : RepositoryBase<CONTA_BANCO>, IContaBancariaRepository
    {
        public CONTA_BANCO GetItemById(Int32 id)
        {
            IQueryable<CONTA_BANCO> query = Db.CONTA_BANCO;
            query = query.Where(p => p.COBA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CONTA_BANCO> GetAllItens(Int32 idUsu)
        {
            IQueryable<CONTA_BANCO> query = Db.CONTA_BANCO.Where(p => p.COBA_IN_ATIVO == 1);
            //query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

    }
}
 