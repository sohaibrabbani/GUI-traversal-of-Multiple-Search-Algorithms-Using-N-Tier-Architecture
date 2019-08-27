:- use_module(library(lists)).

 first([H|_],Node,Hue):-
member(Hue-Node,[H]).
 
bestfs(Path, Steps) :- 
retractall(steps(flow,_)),
assert(steps(flow,[])),
start(S), 
bestfsPath(S,Path),
bestfs(S,Steps,[[]]).

bestfsPath(S,[S]) :- 
goal(S), !.

bestfsPath(S,[S|Path]) :-
findall(Dist-SS,(arc(S,SS), h(SS,Dist)),L),
keysort(L,SortedL),
member(_-NextS,SortedL),
bestfsPath(NextS,Path) .

bestfs(S,Steps,_) :-
goal(S), !->retract(steps(flow,Steps)).

bestfs(S,Steps,[_|List]) :-
findall(Dist-SS,(arc(S,SS), h(SS,Dist)),L), 
append(L,List,List1),
keysort(List1,SortedL),
first(SortedL,Node,Hue),
retract(steps(flow,Flow)),
append(Flow,[Hue-Node],NewFlow),
assert(steps(flow,NewFlow)),
bestfs(Node, Steps, SortedL).