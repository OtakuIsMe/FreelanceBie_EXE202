import React from 'react'
import './Tag.css'

interface Tag{
    tag: string;
}
const Tag : React.FC<Tag> = ({tag}) => {
  return (
    <div id='tag'>
        {tag}
    </div>
  )
}

export default Tag