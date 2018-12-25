#include "EigenValue.h"


float SmallLanda(float a, float b, float c) {
  /* returns smallest root, given that b is negative */
  float Landa1=0.0F;
  float Landa2=0.0F;
  float Discriminant=0.0F; 
  
  Discriminant = b*b - 4*a*c;
  if(Discriminant <= 0.0F )   return 0.0F;
  else{
	      Landa1 = (float)(-b + (float)sqrt(Discriminant)) / (2*a);  
		  Landa2 = (float)(-b - (float)sqrt(Discriminant)) / (2*a);  
		  if(Landa1 > 0.0F && Landa2 > 0.0F){
		    if(Landa1 <= Landa2)
			    return Landa1;
		    else  
				return Landa2;
		  }
		  else 
			return 0.0F;
   };   
}

/* função auxiliar: cria e inicializa um nó */
Lista* cria (float v, int x , int y)
{
	Lista* p = (Lista*) malloc(sizeof(Lista));
	p->info = v;
	p->x = x;
	p->y = y;
	return p;
}

/* função de inicialização: retorna uma lista vazia */
Lista* inicializa (void)
{
    return NULL;
}

/* função libera: libera memoria da lista */
void libera (Lista* l)
{
	Lista* p = l;
	while (p != NULL) {
		Lista* t = p->prox; /* guarda referência para o próximo elemento*/
		free(p); /* libera a memória apontada por p */
		p = t; /* faz p apontar para o próximo */
	}
}


/*******************************************************/
// Functions for get eigen value
/*******************************************************/
void eigen(float q1, float q2, float q3, float q4, float * v) {

  /*
   * Takes matrix [[q1 q2][q3 q4]]. Eigenvector with highest
   * eigenvalue goes in v.
   */

  /* operation is (A - Iv == 0) */

  float a, b, c;

  a = 1.0;
  b = -(q1 + q4);		/* always negative */
  c = (q1 * q4 - q2 * q3);

  *v = SmallLanda(a, b, c);

  return;
}

/* função insere_ordenado: insere elemento em ordem */
Lista* insere_ordenado (Lista *l, float v , int x, int y)
{
	
    Lista* novo = cria(v,x,y); /* cria novo nó */
	Lista* ant = NULL; /* ponteiro para elemento anterior */
	Lista* p = inicializa();
	p = l; /* ponteiro para percorrer a lista*/
	/* procura posição de inserção */
	while (p != NULL && p->info >= v) {
		ant = p;
		p = p->prox;
	}
	/* insere elemento */
	if (ant == NULL) { /* insere elemento no início */
		novo->prox = l;
		l = novo;
	}
	else { /* insere elemento no meio da lista */
		novo->prox = ant->prox;
		ant->prox = novo;
	}
	return l;
}

/* função busca: busca um elemento na lista */
int busca_x(Lista* l, int v)
{
	Lista* p;
        int n=0; 
	for (p=l; p!=NULL; p=p->prox)
		if (p->x == v)
                    n++;                
	return n; /* não achou o elemento */
}

/* função percorrer: busca um elemento na lista e deixa o apontador nesse elemento */
Lista* percorrer(Lista* l, int v)
{
	Lista* p;
        for (p=l; p!=NULL; p=p->prox)
		if (p->prox->x == v)
                    return p;                
	return NULL; /* não achou o elemento */
}

/* função elimina elemento com x repetido */
Lista* elimina_igual_x(Lista *l){
	
	Lista* ant = NULL; /* ponteiro para elemento anterior */
	Lista* p = l; /* ponteiro para percorrer a lista*/
        Lista* q = NULL;
        
        /* verifica se achou elemento */
        if (p->prox == NULL)
            return l;
        else
        {   
            q = p->prox;
            p->prox = q->prox ;
            free(q);
            l -> prox = p ->prox;
        };
                
    return l;   
}

/* função elimina elemento com x repetido */
void retira_x_iguais(Lista *l){
    int i,Cont;
    Lista *q = NULL; 
    for(q = l; q != NULL; q = q->prox)
    {
        l = q; 
        Cont = busca_x(q, q->x)-1;
        for(i = 0; i < Cont ;i++ )
        {   
            l = percorrer(l, q->x);
            if(l!=NULL) l = elimina_igual_x(l);
			//if(l!=NULL && abs(l->prox->y - q->y) <= 2 ) l = elimina_igual_x(l);
        };
    };
}

/* função busca x proximos: busca numero de elementos x proximos na lista */
int busca_x_Prox(Lista* l, int u, int v)
{
	Lista* p;
        int n=0; 
	for (p=l; p!=NULL; p=p->prox)
            if ( abs(p->x - u) < 8 && abs(p->y - v) < 8 )
                    n++;                
	return n; /* não achou o elemento */
}
/* função percorrer: busca um elemento na lista e deixa o apontador nesse elemento */
Lista* percorrer_x_Prox(Lista* l, int u, int v)
{
	Lista* p;
        for (p=l; p!=NULL; p=p->prox)
		if (abs(p->prox->x - u) < 8 && abs(p->prox->y - v) < 8  )
                    return p;                
	return NULL; /* não achou o elemento */
}

/* função elimina elemento com x proximos */
void retira_x_Prox(Lista *l){
    int i,Cont;
    Lista *q = NULL; 
    for(q = l; q != NULL; q = q->prox)
    {
        l = q; 
        Cont = busca_x_Prox(q, q->x, q->y)-1;
        for(i = 0; i < Cont ;i++ )
        {   
            l = percorrer_x_Prox(l, q->x, q->y);
            if(l!=NULL) l = elimina_igual_x(l);
			//if(l!=NULL && abs(l->prox->y - q->y) <= 2 ) l = elimina_igual_x(l);
        };
    };
}

/* função ordena: em referecnia a X */
Lista* insere_ordenado_X (Lista *l, float v , int x, int y)
{
	Lista* novo = cria(v,x,y); /* cria novo nó */
	Lista* ant = NULL; /* ponteiro para elemento anterior */
	Lista* p = inicializa();
	p = l; /* ponteiro para percorrer a lista*/
	/* procura posição de inserção */
	while (p != NULL && p->x <= x) {
		ant = p;
		p = p->prox;
	};

	/* insere elemento */
	if (ant == NULL) { /* insere elemento no início */
		novo->prox = l;
		l = novo;
	}
	else { /* insere elemento no meio da lista */
		novo->prox = ant->prox;
		ant->prox = novo;
	}
	return l;
}