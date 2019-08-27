
iterativeDeepening(Node,Sol,Steps):-
retractall(iterations(i,_)),
assert(iterations(i,[])),
iterativeDeepening(Node, Sol,0,Steps).

depthfirst2(Node,[Node],_):-
goal(Node).

depthfirst2(Node, [Node|Sol], Maxdepth):-
Maxdepth > 0,
arc(Node,Node1),
Max1 is Maxdepth - 1,
retract(iterations(i,OldList)),
append(OldList,[Node1],NewList),
assert(iterations(i,NewList)),
depthfirst2(Node1, Sol, Max1).

iterativeDeepening(Node, Sol, Depth,Steps):-
Y is Depth+1,depthfirst2(Node, Sol, Y)-> 
retract(iterations(i,OldList)),
append(OldList,[],Steps);
X is Depth+1,nl,
retract(iterations(i,OldList)),
append(OldList,[X],NewList),
assert(iterations(i,NewList)),
iterativeDeepening(Node, Sol, X, Steps).
arc(a,b).
arc(a,c).
arc(b,d).
goal(d).
