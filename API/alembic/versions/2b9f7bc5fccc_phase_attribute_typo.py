"""Phase attribute typo

Revision ID: 2b9f7bc5fccc
Revises: 263ed029d6fa
Create Date: 2023-06-17 18:17:51.701233

"""
from alembic import op
import sqlalchemy as sa


# revision identifiers, used by Alembic.
revision = '2b9f7bc5fccc'
down_revision = '263ed029d6fa'
branch_labels = None
depends_on = None


def upgrade() -> None:
    # ### commands auto generated by Alembic - please adjust! ###
    op.add_column('phases', sa.Column('first_alternative_character', sa.String(), nullable=True))
    op.drop_column('phases', 'firs_alternative_character')
    # ### end Alembic commands ###


def downgrade() -> None:
    # ### commands auto generated by Alembic - please adjust! ###
    op.add_column('phases', sa.Column('firs_alternative_character', sa.VARCHAR(), autoincrement=False, nullable=True))
    op.drop_column('phases', 'first_alternative_character')
    # ### end Alembic commands ###
