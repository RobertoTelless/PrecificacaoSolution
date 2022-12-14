using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class ProdutoEstoqueFilialService : ServiceBase<PRODUTO_ESTOQUE_EMPRESA>, IProdutoEstoqueFilialService
    {
        private readonly IProdutoEstoqueFilialRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        protected Db_PrecificacaoEntities Db = new Db_PrecificacaoEntities();

        public ProdutoEstoqueFilialService(IProdutoEstoqueFilialRepository baseRepository, ILogRepository logRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;

        }

        public List<PRODUTO_ESTOQUE_EMPRESA> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public PRODUTO_ESTOQUE_EMPRESA CheckExist(PRODUTO_ESTOQUE_EMPRESA prod, Int32 idAss)
        {
            PRODUTO_ESTOQUE_EMPRESA item = _baseRepository.CheckExist(prod, idAss);
            return item;
        }

        public PRODUTO_ESTOQUE_EMPRESA GetItemById(Int32 id)
        {
            PRODUTO_ESTOQUE_EMPRESA item = _baseRepository.GetItemById(id);
            return item;
        }

        public PRODUTO_ESTOQUE_EMPRESA GetItemById(PRODUTO item)
        {
            PRODUTO_ESTOQUE_EMPRESA obj = _baseRepository.GetItemById(item);
            return obj;
        }

        public List<PRODUTO_ESTOQUE_EMPRESA> GetByProd(Int32 id)
        {
            return _baseRepository.GetByProd(id);
        }

        public PRODUTO_ESTOQUE_EMPRESA GetByProdFilial(Int32 prod, Int32 fili)
        {
            PRODUTO_ESTOQUE_EMPRESA item = _baseRepository.GetByProdFilial(prod, fili);
            return item;
        }

        public Int32 Create(PRODUTO_ESTOQUE_EMPRESA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Edit(PRODUTO_ESTOQUE_EMPRESA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PRODUTO_ESTOQUE_EMPRESA obj = _baseRepository.GetById(item.PREE_CD_ID);
                    _baseRepository.Detach(obj);
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Delete(PRODUTO_ESTOQUE_EMPRESA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _baseRepository.Remove(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
