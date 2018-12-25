#include <stdlib.h>
#include <stdio.h>
#include <conio.h>
#include <math.h>
#include <assert.h>


struct lista {
	float info;
	int x;
	int y;
	struct lista* prox;
	};

typedef struct lista Lista;

/* função de inicialização: retorna uma lista vazia */

Lista* inicializa (void);
void libera (Lista* l);


void eigen(float q1, float q2, float q3, float q4, float * v) ;
  /* returns less root, given that small landa  */




/* função insere_ordenado: insere elemento em ordem */
Lista* insere_ordenado (Lista *l, float v , int x, int y);

/* função elimina elemento com x repetido */
void retira_x_iguais(Lista *l);

/* função elimina elemento com x proximos */
void retira_x_Prox(Lista *l);

/* função ordena: em referecnia a X */
Lista* insere_ordenado_X (Lista *l, float v , int x, int y);