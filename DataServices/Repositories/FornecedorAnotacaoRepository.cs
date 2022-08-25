using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class FornecedorAnotacaoRepository : RepositoryBase<FORNECEDOR_ANOTACOES>, IFornecedorAnotacaoRepository
    {
        public List<FORNECEDOR_ANOTACOES> GetAllItens()
        {
            return Db.FORNECEDOR_ANOTACOES.ToList();
        }

        public FORNECEDOR_ANOTACOES GetItemById(Int32 id)
        {
            IQueryable<FORNECEDOR_ANOTACOES> query = Db.FORNECEDOR_ANOTACOES.Where(p => p.FOAT_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 