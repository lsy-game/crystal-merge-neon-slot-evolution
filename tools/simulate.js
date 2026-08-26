"use strict";

// 与 game.js 的新手转轴保持同一组权重和赔付，用于快速检查长期实际RTP。
const symbols = {
  wheat:{weight:27,pays:{3:2,4:5,5:12},normal:true}, apple:{weight:23,pays:{3:3,4:7,5:16},normal:true},
  milk:{weight:19,pays:{3:4,4:9,5:22},normal:true}, bread:{weight:15,pays:{3:6,4:14,5:35},normal:true},
  gem:{weight:10,pays:{3:10,4:26,5:70},normal:true}, wild:{weight:4,pays:{3:12,4:35,5:100},wild:true},
  scatter:{weight:2,scatter:true},
};
const lines=[[0,0,0,0,0],[1,1,1,1,1],[2,2,2,2,2],[0,1,2,1,0],[2,1,0,1,2],[0,0,1,2,2],[2,2,1,0,0],[1,0,0,0,1],[1,2,2,2,1],[0,1,1,1,0],[2,1,1,1,2],[1,0,1,2,1],[1,2,1,0,1],[0,1,0,1,0],[2,1,2,1,2],[0,2,0,2,0],[2,0,2,0,2],[0,2,2,2,0],[2,0,0,0,2],[0,0,2,0,0],[2,2,0,2,2],[1,0,2,0,1],[1,2,0,2,1],[0,1,2,2,2],[2,1,0,0,0]];
const entries=Object.entries(symbols), totalWeight=entries.reduce((n,[,s])=>n+s.weight,0), at=(c,r)=>c*3+r;
function pick(){let n=Math.random()*totalWeight;for(const [id,s] of entries){n-=s.weight;if(n<=0)return id}return"wheat"}
function grow(grid){const visited=new Set();for(let start=0;start<15;start++){if(visited.has(start)||!symbols[grid[start]].normal)continue;const type=grid[start],group=[],q=[start];visited.add(start);while(q.length){const p=q.shift(),c=Math.floor(p/3),r=p%3;group.push(p);for(const[nc,nr]of[[c-1,r],[c+1,r],[c,r-1],[c,r+1]]){const i=at(nc,nr);if(nc>=0&&nc<5&&nr>=0&&nr<3&&!visited.has(i)&&grid[i]===type){visited.add(i);q.push(i)}}}if(group.length<3)continue;const candidates=[];for(const p of group){const c=Math.floor(p/3),r=p%3;for(const[nc,nr]of[[c-1,r],[c+1,r],[c,r-1],[c,r+1]]){const i=at(nc,nr);if(nc>=0&&nc<5&&nr>=0&&nr<3&&!group.includes(i)&&symbols[grid[i]].normal&&!candidates.includes(i))candidates.push(i)}}for(const i of candidates.sort(()=>Math.random()-.5).slice(0,2)){grid[i]=type}break}}
function evaluate(grid){let payout=0;for(const rows of lines){const ids=rows.map((r,c)=>grid[at(c,r)]);let target=ids.find(id=>id!=="wild"&&id!=="scatter");if(!target&&ids[0]==="wild")target="wild";if(!target||ids[0]==="scatter")continue;let count=0;for(const id of ids){if(id===target||id==="wild")count++;else break}if(count>=3)payout+=symbols[target].pays[count]||0}return {payout,scatters:grid.filter(id=>id==="scatter").length}}
const rounds=Number(process.argv[2]||1_000_000),scale=Number(process.argv[3]||.385);let paid=0,basePaid=0,hits=0,lossRun=0,guarantees=0,guaranteePaid=0,freeSpins=0,totalSpins=0;
for(let n=0;n<rounds;n++){let queue=1;while(queue--){totalSpins++;const grid=Array.from({length:15},pick);grow(grid);const result=evaluate(grid);let win=Math.round(result.payout*scale);basePaid+=win;if(win<10)lossRun++;else lossRun=0;if(lossRun>=6){const grant=Math.max(18,22-win);win+=grant;guaranteePaid+=grant;guarantees++;lossRun=0}if(result.scatters>=3){const award=5+Math.max(0,result.scatters-3)*2;queue+=award;freeSpins+=award}paid+=win;if(win>0)hits++}}
console.log(JSON.stringify({paidRounds:rounds,totalSpins,freeSpins,scale,bet:rounds*10,baseRtp:(basePaid/(rounds*10)*100).toFixed(2)+"%",guarantees,guaranteeContribution:(guaranteePaid/(rounds*10)*100).toFixed(2)+"%",paid,rtp:(paid/(rounds*10)*100).toFixed(2)+"%",hitRate:(hits/totalSpins*100).toFixed(2)+"%"},null,2));
