using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ProdutoEstoqueFilialRepository : RepositoryBase<PRODUTO_ESTOQUE_EMPRESA>, IProdutoEstoqueFilialRepository
    {
        public List<PRODUTO_ESTOQUE_EMPRESA> GetAllItens(Int32 idAss)
        {
            IQueryable<PRODUTO_ESTOQUE_EMPRESA> query = Db.PRODUTO_ESTOQUE_EMPRESA;
            return query.ToList<PRODUTO_ESTOQUE_EMPRESA>();
        }

        public PRODUTO_ESTOQUE_EMPRESA GetByProdFilial(Int32 prod, Int32 fili)
        {
            IQueryable<PRODUTO_ESTOQUE_EMPRESA> query = Db.PRODUTO_ESTOQUE_EMPRESA;
            query = query.Where(p => p.PROD_CD_ID == prod);
            query = query.Where(p => p.EMPR_CD_ID == fili);
            return query.FirstOrDefault();
        }

        public List<PRODUTO_ESTOQUE_EMPRESA> GetByProd(Int32 id)
        {
            IQueryable<PRODUTO_ESTOQUE_EMPRESA> query = Db.PRODUTO_ESTOQUE_EMPRESA;
            query = query.Where(p => p.PROD_CD_ID == id);
            return query.ToList<PRODUTO_ESTOQUE_EMPRESA>();
        }

        public PRODUTO_ESTOQUE_EMPRESA CheckExist(PRODUTO_ESTOQUE_EMPRESA item, Int32 idAss)
        {
            IQueryable<PRODUTO_ESTOQUE_EMPRESA> query = Db.PRODUTO_ESTOQUE_EMPRESA;
            query = query.Where(p => p.PROD_CD_ID == item.PROD_CD_ID);
            query = query.Where(p => p.EMPR_CD_ID == item.EMPR_CD_ID);
            return query.FirstOrDefault();
        }

        public PRODUTO_ESTOQUE_EMPRESA GetItemById(Int32 id)
        {
            IQueryable<PRODUTO_ESTOQUE_EMPRESA> query = Db.PRODUTO_ESTOQUE_EMPRESA;
            query = query.Where(p => p.PREE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public PRODUTO_ESTOQUE_EMPRESA GetItemById(PRODUTO item)
        {
            IQueryable<PRODUTO_ESTOQUE_EMPRESA> query = Db.PRODUTO_ESTOQUE_EMPRESA;
            query = query.Where(p => p.PROD_CD_ID == item.PROD_CD_ID).OrderByDescending(x => x.PREE_DT_ULTIMO_MOVIMENTO);
            return query.FirstOrDefault();
        }
    }
}
