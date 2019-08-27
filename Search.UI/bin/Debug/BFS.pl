:- dynamic steps/2.


bfs(S,Path,Steps) :-
retractall(steps(flow,_)),
assert(steps(flow,[])),
append([],[],Q1),
append([S],Q1,Q2),
bfs1(Q2,Path,Steps).


bfs1(Q,[G,S|Tail],Steps) :-
(append([S|Tail],_,Q),
arc(S,G), 
goal(G)),
%findall(Succ, (arc(S,Succ)),NewPaths),
retract(steps(flow,Flow)),
%append(Flow,NewPaths,NewFlow),
%assert(steps(flow,NewFlow)),
append([G],Flow,Steps).


bfs1(Q1,Solution,Steps) :-
append([S|Tail],Q2,Q1),
findall([Succ,S], (arc(S,Succ), \+member(Succ,Tail)),NewPaths),
findall(Curr, (arc(S,Curr), \+member(Succ,Tail)),Currpath),
append(Q2,NewPaths,Q3),
retract(steps(flow,Flow)),
append(Flow,Currpath,NewFlow),
assert(steps(flow,NewFlow)),
bfs1(Q3,Solution,Steps).

arc([H|_],B):-
arc(H,B).

arc(a,b).
arc(a,c).
goal(c).
